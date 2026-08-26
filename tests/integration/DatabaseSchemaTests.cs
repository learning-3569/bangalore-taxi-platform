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
        var booking = CreateGuestBooking("BLR-2026-000010", assigned: false);
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
        db.Bookings.Add(CreateGuestBooking("BLR-2026-000011", assigned: false));
        await db.SaveChangesAsync();

        await using var db2 = _postgres.CreateContext();
        db2.Bookings.Add(CreateGuestBooking("BLR-2026-000011", assigned: false));
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
            "BLR-2026-000020",
            assigned: true,
            driverId: driver.Id,
            vehicleId: vehicle.Id,
            pickup: new DateTimeOffset(2026, 8, 25, 10, 0, 0, TimeSpan.Zero),
            end: new DateTimeOffset(2026, 8, 25, 12, 0, 0, TimeSpan.Zero)));
        await db.SaveChangesAsync();

        await using var db2 = _postgres.CreateContext();
        db2.Bookings.Add(CreateGuestBooking(
            "BLR-2026-000021",
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
        var (driver, vehicleA) = await CreateDriverAndVehicleAsync(setup, "KA01AB1001");
        var vehicleB = new Vehicle
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = "KA01AB1002",
            VehicleTypeId = ReferenceData.VehicleTypeIds.Sedan,
            Capacity = 4,
            StatusId = ReferenceData.VehicleStatusActive
        };
        setup.Vehicles.Add(vehicleB);
        await setup.SaveChangesAsync();

        await using var db = _postgres.CreateContext();
        db.Bookings.Add(CreateGuestBooking(
            "BLR-2026-000030",
            assigned: true,
            driverId: driver.Id,
            vehicleId: vehicleA.Id,
            pickup: new DateTimeOffset(2026, 8, 26, 10, 0, 0, TimeSpan.Zero),
            end: new DateTimeOffset(2026, 8, 26, 12, 0, 0, TimeSpan.Zero)));
        await db.SaveChangesAsync();

        await using var db2 = _postgres.CreateContext();
        db2.Bookings.Add(CreateGuestBooking(
            "BLR-2026-000031",
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
        db.AuditLogs.Add(new AuditLog
        {
            Action = "booking.accept",
            EntityType = "booking",
            EntityId = Guid.NewGuid(),
            IpAddress = IPAddress.Parse("203.0.113.10"),
            CreatedAt = DateTimeOffset.UtcNow
        });
        db.AuditLogs.Add(new AuditLog
        {
            Action = "booking.assign",
            EntityType = "booking",
            EntityId = Guid.NewGuid(),
            IpAddress = IPAddress.Parse("2001:db8::1"),
            CreatedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync();

        Assert.Equal(2, await db.AuditLogs.CountAsync());
    }

    private static async Task<(Driver Driver, Vehicle Vehicle)> CreateDriverAndVehicleAsync(
        BangaloreTaxiDbContext db,
        string registration = "KA01AB1234")
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            PhoneE164 = "+91" + Guid.NewGuid().ToString("N")[..10],
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
            RegistrationNumber = registration,
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
            booking.AssignedVehicleRegistration = "KA01AB1234";
            booking.AssignedVehicleTypeCode = "sedan";
            booking.AssignedVehicleTypeName = "Sedan";
            var from = DateTime.SpecifyKind(pickupAt.UtcDateTime, DateTimeKind.Utc);
            var to = DateTime.SpecifyKind(endAt.UtcDateTime, DateTimeKind.Utc);
            booking.AssignmentWindow = new NpgsqlRange<DateTime>(from, true, to, false);
        }

        return booking;
    }

    private void RequireDatabase()
    {
        Skip.If(!_postgres.IsAvailable, "PostgreSQL is required (Docker or SCHEMA_TEST_CONNECTION).");
    }
}
