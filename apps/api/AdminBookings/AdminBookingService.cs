using System.Net;
using System.Data;
using System.Text.Json;
using BangaloreTaxi.Api.Application;
using BangaloreTaxi.Api.Bookings;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using BangaloreTaxi.Api.Configuration;

namespace BangaloreTaxi.Api.AdminBookings;

public sealed class AdminBookingService(BangaloreTaxiDbContext db, TimeProvider clock, IOptions<OperationsOptions> operations)
{
    private const int DefaultPageSize = 25;
    private const int MaxPageSize = 100;
    private static readonly Dictionary<string, short> Statuses = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pending"] = ReferenceData.BookingStatusPending,
        ["accepted"] = ReferenceData.BookingStatusAccepted,
        ["rejected"] = ReferenceData.BookingStatusRejected,
        ["driver_assigned"] = ReferenceData.BookingStatusDriverAssigned,
        ["confirmed"] = ReferenceData.BookingStatusConfirmed,
        ["driver_en_route"] = ReferenceData.BookingStatusDriverEnRoute,
        ["picked_up"] = ReferenceData.BookingStatusPickedUp,
        ["completed"] = ReferenceData.BookingStatusCompleted,
        ["cancelled"] = ReferenceData.BookingStatusCancelled
    };

    public async Task<AdminBookingPage> ListAsync(string? status, int page, int pageSize, CancellationToken ct)
    {
        if (page < 1) throw new InvalidRequestException("Page must be at least 1.");
        if (pageSize == 0) pageSize = DefaultPageSize;
        if (pageSize is < 1 or > MaxPageSize) throw new InvalidRequestException($"Page size must be between 1 and {MaxPageSize}.");

        var query = db.Bookings.AsNoTracking().Include(x => x.Status).Include(x => x.RequestedVehicleType).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Statuses.TryGetValue(status.Trim(), out var statusId)) throw new InvalidRequestException("Choose a valid booking status.");
            query = query.Where(x => x.StatusId == statusId);
        }

        var total = await query.CountAsync(ct);
        var rows = await query.OrderBy(x => x.PickupAt).ThenBy(x => x.CreatedAt).ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new AdminBookingPage(rows.Select(MapList).ToList(), page, pageSize, total,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize));
    }

    public async Task<AdminBookingDetails> GetAsync(Guid id, CancellationToken ct) =>
        Map(await DetailsQuery().SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Booking was not found."));

    public Task<AdminBookingDetails> AcceptAsync(Guid adminId, Guid id, IPAddress? ip, CancellationToken ct) =>
        TransitionAsync(adminId, id, ReferenceData.BookingStatusAccepted, "accepted", "Booking request accepted", null, ip, ct);

    public Task<AdminBookingDetails> RejectAsync(Guid adminId, Guid id, string reason, IPAddress? ip, CancellationToken ct)
    {
        reason = Required(reason, 300);
        return TransitionAsync(adminId, id, ReferenceData.BookingStatusRejected, "rejected",
            "Booking request not accepted", reason, ip, ct);
    }

    public async Task<AdminBookingDetails> AssignAsync(
        Guid adminId, Guid id, Guid driverId, Guid vehicleId, IPAddress? ip, CancellationToken ct)
    {
        if (driverId == Guid.Empty || vehicleId == Guid.Empty)
            throw new InvalidRequestException("Choose a driver and vehicle.");

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            var booking = await db.Bookings.SingleOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new NotFoundException("Booking was not found.");
            if (booking.StatusId != ReferenceData.BookingStatusAccepted || booking.AssignedDriverId is not null || booking.AssignedVehicleId is not null)
                throw new ConflictException("Only an unassigned accepted booking can be assigned.");

            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM driver WHERE id = {driverId} FOR UPDATE", ct);
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM vehicle WHERE id = {vehicleId} FOR UPDATE", ct);
            var driver = await db.Drivers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == driverId, ct)
                ?? throw new InvalidRequestException("Choose a valid driver.");
            var vehicle = await db.Vehicles.Include(x => x.VehicleType).SingleOrDefaultAsync(x => x.Id == vehicleId, ct)
                ?? throw new InvalidRequestException("Choose a valid vehicle.");

            if (driver.EmploymentStatusId != ReferenceData.DriverEmploymentActive
                || driver.AvailabilityStatusId != ReferenceData.DriverAvailabilityAvailable
                || driver.User.StatusId != ReferenceData.UserStatusActive || string.IsNullOrWhiteSpace(driver.User.PhoneE164))
                throw new ConflictException("The selected driver is not currently eligible for assignment.");
            if (vehicle.StatusId != ReferenceData.VehicleStatusActive || !vehicle.VehicleType.IsActive)
                throw new ConflictException("The selected vehicle is not currently eligible for assignment.");
            if (vehicle.VehicleTypeId != booking.RequestedVehicleTypeId)
                throw new ConflictException("The selected vehicle type does not match the requested category.");

            var settings = await db.OperationalSettings.Where(x => x.Key == ReferenceData.AssignmentBufferMinutesKey
                    || x.Key == ReferenceData.DefaultTripDurationMinutesKey).ToDictionaryAsync(x => x.Key, x => x.Value, ct);
            var bufferMinutes = Setting(settings, ReferenceData.AssignmentBufferMinutesKey, operations.Value.AssignmentBufferMinutes);
            var durationMinutes = Setting(settings, ReferenceData.DefaultTripDurationMinutesKey, operations.Value.DefaultTripDurationMinutes);
            var tripStart = booking.PickupAt;
            var finalLegStart = booking.ReturnAt ?? booking.PickupAt;
            var estimatedEnd = finalLegStart.AddMinutes(durationMinutes);
            var lower = tripStart.AddMinutes(-bufferMinutes).UtcDateTime;
            var upper = estimatedEnd.AddMinutes(bufferMinutes).UtcDateTime;

            booking.AssignedDriverId = driver.Id;
            booking.AssignedDriverDisplayName = driver.DisplayName;
            booking.AssignedDriverPhoneE164 = driver.User.PhoneE164;
            booking.AssignedVehicleId = vehicle.Id;
            booking.AssignedVehicleRegistration = vehicle.RegistrationNumber;
            booking.AssignedVehicleTypeCode = vehicle.VehicleType.Code;
            booking.AssignedVehicleTypeName = vehicle.VehicleType.Name;
            booking.EstimatedEndAt = estimatedEnd;
            booking.AssignmentWindow = new NpgsqlRange<DateTime>(lower, true, upper, false);
            booking.StatusId = ReferenceData.BookingStatusDriverAssigned;

            var now = clock.GetUtcNow();
            db.BookingStatusHistories.Add(new BookingStatusHistory
            {
                BookingId = booking.Id, FromStatusId = ReferenceData.BookingStatusAccepted,
                ToStatusId = ReferenceData.BookingStatusDriverAssigned, ChangedByUserId = adminId,
                Reason = "Driver and vehicle assigned", CreatedAt = now
            });
            db.AuditLogs.Add(new AuditLog
            {
                ActorUserId = adminId, Action = "booking_assigned", EntityType = "booking", EntityId = booking.Id,
                OldValue = JsonSerializer.Serialize(new { status = "accepted" }),
                NewValue = JsonSerializer.Serialize(new { status = "driver_assigned", driverId, vehicleId }),
                IpAddress = ip, CreatedAt = now
            });
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return await GetAsync(id, ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(ct);
            throw new ConflictException("The booking changed. Refresh and try again.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ExclusionViolation })
        {
            await transaction.RollbackAsync(ct);
            throw new ConflictException("The driver or vehicle is already assigned during this booking window.");
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<AdminBookingDetails> TransitionAsync(
        Guid adminId, Guid id, short targetStatusId, string targetCode, string customerReason,
        string? internalReason, IPAddress? ip, CancellationToken ct)
    {
        var booking = await db.Bookings.SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Booking was not found.");
        if (booking.StatusId != ReferenceData.BookingStatusPending)
            throw new ConflictException("Only a pending booking request can be accepted or rejected.");

        var now = clock.GetUtcNow();
        booking.StatusId = targetStatusId;
        db.BookingStatusHistories.Add(new BookingStatusHistory
        {
            BookingId = booking.Id, FromStatusId = ReferenceData.BookingStatusPending,
            ToStatusId = targetStatusId, ChangedByUserId = adminId, Reason = customerReason, CreatedAt = now
        });
        db.AuditLogs.Add(new AuditLog
        {
            ActorUserId = adminId, Action = $"booking_{targetCode}", EntityType = "booking", EntityId = booking.Id,
            OldValue = JsonSerializer.Serialize(new { status = "pending" }),
            NewValue = JsonSerializer.Serialize(new { status = targetCode, reason = internalReason }),
            IpAddress = ip, CreatedAt = now
        });
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { throw new ConflictException("The booking changed. Refresh and try again."); }
        return await GetAsync(id, ct);
    }

    private IQueryable<Booking> DetailsQuery() => db.Bookings.AsNoTracking()
        .Include(x => x.Status).Include(x => x.TripType).Include(x => x.JourneyType)
        .Include(x => x.RequestedVehicleType).Include(x => x.StatusHistory).ThenInclude(x => x.FromStatus)
        .Include(x => x.StatusHistory).ThenInclude(x => x.ToStatus);

    private static AdminBookingListItem MapList(Booking x) => new(
        x.Id, x.BookingNumber, x.Status.Code, BookingService.Label(x.Status.Code), x.PickupAddress,
        x.DropAddress ?? "", x.PickupAt, x.PickupTimeZone, x.PickupLocalDate,
        x.RequestedVehicleType.Code, x.RequestedVehicleType.Name, x.CreatedAt);

    private static AdminBookingDetails Map(Booking x) => new(
        x.Id, x.BookingNumber, x.Status.Code, BookingService.Label(x.Status.Code),
        x.TripTypeId == ReferenceData.TripTypeCorporate ? "hourly" : x.TripType.Code,
        AirportJourney(x), x.PickupAddress, x.DropAddress ?? "", x.PickupAt, x.PickupTimeZone,
        x.PickupLocalDate, x.ReturnAt, x.ReturnLocalDate, x.RequestedVehicleType.Code,
        x.RequestedVehicleType.Name, x.ContactName, x.ContactMobileE164, x.ContactEmail,
        x.CustomerNotes, x.CreatedAt, x.StatusId == ReferenceData.BookingStatusPending,
        x.StatusId == ReferenceData.BookingStatusPending, x.StatusId == ReferenceData.BookingStatusAccepted
            && x.AssignedDriverId is null && x.AssignedVehicleId is null,
        x.AssignedDriverDisplayName, x.AssignedVehicleRegistration, x.AssignedVehicleTypeName,
        x.StatusHistory.OrderBy(h => h.CreatedAt)
            .Select(h => new AdminBookingHistory(h.FromStatus?.Code, h.ToStatus.Code,
                BookingService.Label(h.ToStatus.Code), h.CreatedAt, h.Reason)).ToList());

    private static string? AirportJourney(Booking x) => x.TripTypeId != ReferenceData.TripTypeAirport ? null
        : x.JourneyTypeId == ReferenceData.JourneyTypeRoundTrip ? AirportBookingRules.RoundTripJourney
        : x.PickupAddress == AirportBookingRules.CanonicalLocation ? AirportBookingRules.PickupJourney
        : AirportBookingRules.DropJourney;

    private static string Required(string value, int max) => string.IsNullOrWhiteSpace(value)
        ? throw new InvalidRequestException("Rejection reason is required.")
        : value.Trim().Length > max ? throw new InvalidRequestException("Rejection reason is too long.") : value.Trim();

    private static int Setting(IReadOnlyDictionary<string, string> values, string key, int fallback) =>
        values.TryGetValue(key, out var value) && int.TryParse(value, out var parsed) && parsed > 0 ? parsed : fallback;
}
