using System.Net;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace BangaloreTaxi.IntegrationTests;

[Collection("postgres")]
public sealed class DatabaseSchemaTests
{
    private readonly PostgresFixture _postgres;

    public DatabaseSchemaTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    [SkippableFact]
    public async Task Migration_seeds_lookup_and_operational_data()
    {
        RequireDatabase();
        await using var db = _postgres.CreateContext();

        Assert.Equal(3, await db.Roles.CountAsync());
        Assert.Equal(4, await db.VehicleTypes.CountAsync());
        Assert.Equal(9, await db.BookingStatuses.CountAsync());
        Assert.Equal(3, await db.DriverEmploymentStatuses.CountAsync());
        Assert.Equal(4, await db.DriverAvailabilityStatuses.CountAsync());
        Assert.Equal("15", await db.OperationalSettings
            .Where(s => s.Key == ReferenceData.AssignmentBufferMinutesKey)
            .Select(s => s.Value)
            .SingleAsync());
        Assert.Equal("120", await db.OperationalSettings
            .Where(s => s.Key == ReferenceData.DefaultTripDurationMinutesKey)
            .Select(s => s.Value)
            .SingleAsync());
    }

    [SkippableFact]
    public async Task Guest_booking_can_be_stored_without_customer()
    {
        RequireDatabase();
        await using var db = _postgres.CreateContext();
        var booking = CreateGuestBooking(UniqueBookingNumber(), assigned: false);
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        var stored = await db.Bookings.SingleAsync(b => b.Id == booking.Id);
        Assert.Null(stored.CustomerId);
        Assert.Equal("Guest Traveller", stored.ContactName);
        Assert.Null(stored.PickupLatitude);
    }

    [SkippableFact]
    public async Task Duplicate_booking_number_is_rejected()
    {
        RequireDatabase();
        await using var db = _postgres.CreateContext();
        var bookingNumber = UniqueBookingNumber();
        db.Bookings.Add(CreateGuestBooking(bookingNumber, assigned: false));
        await db.SaveChangesAsync();

        await using var db2 = _postgres.CreateContext();
        db2.Bookings.Add(CreateGuestBooking(bookingNumber, assigned: false));
        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db2.SaveChangesAsync());
        Assert.IsType<PostgresException>(ex.InnerException);
        Assert.Equal(PostgresErrorCodes.UniqueViolation, ((PostgresException)ex.InnerException!).SqlState);
    }

    [SkippableFact]
    public async Task Overlapping_vehicle_assignment_is_rejected()
    {
        RequireDatabase();
        await using var setup = _postgres.CreateContext();
        var (driver, vehicle) = await CreateDriverAndVehicleAsync(setup);

        await using var db = _postgres.CreateContext();
        db.Bookings.Add(CreateGuestBooking(
            UniqueBookingNumber(),
            assigned: true,
            driverId: driver.Id,
            vehicleId: vehicle.Id,
            pickup: new DateTimeOffset(2026, 8, 25, 10, 0, 0, TimeSpan.Zero),
            end: new DateTimeOffset(2026, 8, 25, 12, 0, 0, TimeSpan.Zero)));
        await db.SaveChangesAsync();

        await using var db2 = _postgres.CreateContext();
        db2.Bookings.Add(CreateGuestBooking(
            UniqueBookingNumber(),
            assigned: true,
            driverId: driver.Id,
            vehicleId: vehicle.Id,
            pickup: new DateTimeOffset(2026, 8, 25, 10, 30, 0, TimeSpan.Zero),
            end: new DateTimeOffset(2026, 8, 25, 12, 30, 0, TimeSpan.Zero)));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db2.SaveChangesAsync());
        var pg = Assert.IsType<PostgresException>(ex.InnerException);
        Assert.Equal(PostgresErrorCodes.ExclusionViolation, pg.SqlState);
    }

    [SkippableFact]
    public async Task Overlapping_driver_assignment_on_different_vehicles_is_rejected()
    {
        RequireDatabase();
        await using var setup = _postgres.CreateContext();
        var (driver, vehicleA) = await CreateDriverAndVehicleAsync(setup);
        var vehicleB = new Vehicle
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = UniqueRegistration(),
            VehicleTypeId = ReferenceData.VehicleTypeIds.Sedan,
            Capacity = 4,
            StatusId = ReferenceData.VehicleStatusActive
        };
        setup.Vehicles.Add(vehicleB);
        await setup.SaveChangesAsync();

        await using var db = _postgres.CreateContext();
        db.Bookings.Add(CreateGuestBooking(
            UniqueBookingNumber(),
            assigned: true,
            driverId: driver.Id,
            vehicleId: vehicleA.Id,
            pickup: new DateTimeOffset(2026, 8, 26, 10, 0, 0, TimeSpan.Zero),
            end: new DateTimeOffset(2026, 8, 26, 12, 0, 0, TimeSpan.Zero)));
        await db.SaveChangesAsync();

        await using var db2 = _postgres.CreateContext();
        db2.Bookings.Add(CreateGuestBooking(
            UniqueBookingNumber(),
            assigned: true,
            driverId: driver.Id,
            vehicleId: vehicleB.Id,
            pickup: new DateTimeOffset(2026, 8, 26, 10, 30, 0, TimeSpan.Zero),
            end: new DateTimeOffset(2026, 8, 26, 12, 30, 0, TimeSpan.Zero)));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db2.SaveChangesAsync());
        var pg = Assert.IsType<PostgresException>(ex.InnerException);
        Assert.Equal(PostgresErrorCodes.ExclusionViolation, pg.SqlState);
    }

    [SkippableFact]
    public async Task Audit_log_stores_ipv4_and_ipv6()
    {
        RequireDatabase();
        await using var db = _postgres.CreateContext();
        var ipv4EntityId = Guid.NewGuid();
        var ipv6EntityId = Guid.NewGuid();
        db.AuditLogs.Add(new AuditLog
        {
            Action = "booking.accept",
            EntityType = "booking",
            EntityId = ipv4EntityId,
            IpAddress = IPAddress.Parse("203.0.113.10"),
            CreatedAt = DateTimeOffset.UtcNow
        });
        db.AuditLogs.Add(new AuditLog
        {
            Action = "booking.assign",
            EntityType = "booking",
            EntityId = ipv6EntityId,
            IpAddress = IPAddress.Parse("2001:db8::1"),
            CreatedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync();

        var stored = await db.AuditLogs
            .Where(row => row.EntityId == ipv4EntityId || row.EntityId == ipv6EntityId)
            .ToListAsync();
        Assert.Equal(2, stored.Count);
        Assert.Contains(stored, row => IPAddress.Parse("203.0.113.10").Equals(row.IpAddress));
        Assert.Contains(stored, row => IPAddress.Parse("2001:db8::1").Equals(row.IpAddress));
    }

    private static async Task<(Driver Driver, Vehicle Vehicle)> CreateDriverAndVehicleAsync(BangaloreTaxiDbContext db)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            PhoneE164 = UniqueTestPhoneE164(),
            StatusId = ReferenceData.UserStatusActive
        };
        var driver = new Driver
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DisplayName = "Test Driver",
            EmploymentStatusId = ReferenceData.DriverEmploymentActive,
            AvailabilityStatusId = ReferenceData.DriverAvailabilityAvailable
        };
        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = UniqueRegistration(),
            VehicleTypeId = ReferenceData.VehicleTypeIds.Sedan,
            Capacity = 4,
            StatusId = ReferenceData.VehicleStatusActive
        };
        db.Users.Add(user);
        db.Drivers.Add(driver);
        db.Vehicles.Add(vehicle);
        await db.SaveChangesAsync();
        return (driver, vehicle);
    }

    private static Booking CreateGuestBooking(
        string bookingNumber,
        bool assigned,
        Guid? driverId = null,
        Guid? vehicleId = null,
        DateTimeOffset? pickup = null,
        DateTimeOffset? end = null)
    {
        var pickupAt = pickup ?? new DateTimeOffset(2026, 9, 1, 8, 0, 0, TimeSpan.Zero);
        var endAt = end ?? pickupAt.AddHours(2);
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = bookingNumber,
            ContactName = "Guest Traveller",
            ContactMobileE164 = "+919876543210",
            PickupAddress = "Kempegowda International Airport",
            PickupAt = pickupAt,
            PickupTimeZone = "Asia/Kolkata",
            PickupLocalDate = DateOnly.FromDateTime(pickupAt.UtcDateTime),
            RequestedVehicleTypeId = ReferenceData.VehicleTypeIds.Sedan,
            TripTypeId = ReferenceData.TripTypeAirport,
            JourneyTypeId = ReferenceData.JourneyTypeOneWay,
            StatusId = assigned ? ReferenceData.BookingStatusConfirmed : ReferenceData.BookingStatusPending,
            EstimatedFareAmount = 1200.50m,
            CurrencyCode = "INR"
        };

        if (assigned)
        {
            booking.AssignedDriverId = driverId;
            booking.AssignedVehicleId = vehicleId;
            booking.EstimatedEndAt = endAt;
            booking.AssignedDriverDisplayName = "Test Driver";
            booking.AssignedDriverPhoneE164 = "+919811111111";
            booking.AssignedVehicleRegistration = "KA01AB9999";
            booking.AssignedVehicleTypeCode = "sedan";
            booking.AssignedVehicleTypeName = "Sedan";
            var from = DateTime.SpecifyKind(pickupAt.UtcDateTime, DateTimeKind.Utc);
            var to = DateTime.SpecifyKind(endAt.UtcDateTime, DateTimeKind.Utc);
            booking.AssignmentWindow = new NpgsqlRange<DateTime>(from, true, to, false);
        }

        return booking;
    }

    private static long _phoneSequence;
    private static long _bookingSequence = Random.Shared.Next(100_000, 400_000);

    private static string UniqueTestPhoneE164()
    {
        var n = Interlocked.Increment(ref _phoneSequence);
        var eight = (DateTime.UtcNow.Ticks % 10_000_000 * 100 + n % 100) % 100_000_000;
        return "+9198" + eight.ToString("D8");
    }

    private static string UniqueBookingNumber()
    {
        var n = Interlocked.Increment(ref _bookingSequence);
        return BookingNumberFormatter.Format(2026, n);
    }

    private static string UniqueRegistration()
    {
        return "KA" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    }

    private void RequireDatabase()
    {
        Skip.If(!_postgres.IsAvailable, "PostgreSQL is required (Docker or SCHEMA_TEST_CONNECTION).");
    }
}
