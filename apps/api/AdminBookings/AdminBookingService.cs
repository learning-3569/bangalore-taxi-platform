using System.Net;
using System.Text.Json;
using BangaloreTaxi.Api.Application;
using BangaloreTaxi.Api.Bookings;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BangaloreTaxi.Api.AdminBookings;

public sealed class AdminBookingService(BangaloreTaxiDbContext db, TimeProvider clock)
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
        x.StatusId == ReferenceData.BookingStatusPending, x.StatusHistory.OrderBy(h => h.CreatedAt)
            .Select(h => new AdminBookingHistory(h.FromStatus?.Code, h.ToStatus.Code,
                BookingService.Label(h.ToStatus.Code), h.CreatedAt, h.Reason)).ToList());

    private static string? AirportJourney(Booking x) => x.TripTypeId != ReferenceData.TripTypeAirport ? null
        : x.JourneyTypeId == ReferenceData.JourneyTypeRoundTrip ? AirportBookingRules.RoundTripJourney
        : x.PickupAddress == AirportBookingRules.CanonicalLocation ? AirportBookingRules.PickupJourney
        : AirportBookingRules.DropJourney;

    private static string Required(string value, int max) => string.IsNullOrWhiteSpace(value)
        ? throw new InvalidRequestException("Rejection reason is required.")
        : value.Trim().Length > max ? throw new InvalidRequestException("Rejection reason is too long.") : value.Trim();
}
