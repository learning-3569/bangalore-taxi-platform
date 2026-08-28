using System.Data;
using BangaloreTaxi.Api.Application;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace BangaloreTaxi.Api.Bookings;

public sealed class BookingService(BangaloreTaxiDbContext db, TimeProvider clock)
{
    private const string TimeZone = "Asia/Kolkata";
    private static readonly short[] Cancellable =
        [ReferenceData.BookingStatusPending, ReferenceData.BookingStatusAccepted, ReferenceData.BookingStatusConfirmed];

    public async Task<BookingResponse> CreateAsync(Guid userId, CreateBookingRequest input, string? key, CancellationToken ct)
    {
        key = NormalizeKey(key);
        var owner = await db.Customers.Include(x => x.User)
            .SingleOrDefaultAsync(x => x.UserId == userId && x.StatusId == ReferenceData.CustomerStatusActive, ct)
            ?? throw new UnauthorizedException("An active customer account is required.");

        if (key is not null)
        {
            var replay = await Query(owner.Id).SingleOrDefaultAsync(x => x.IdempotencyKey == key, ct);
            if (replay is not null) return Map(replay);
        }

        var requestedServiceType = Required(input.ServiceType, "Service type", 32).ToLowerInvariant();
        var airportJourney = AirportJourney(requestedServiceType, input.AirportJourneyType);
        var pickup = Required(input.Pickup, "Pickup", 500);
        var drop = Required(input.Drop, "Drop", 500);
        if (airportJourney == AirportBookingRules.PickupJourney && pickup != AirportBookingRules.CanonicalLocation)
            throw new InvalidRequestException("Airport Pickup must start at Kempegowda International Airport (BLR).");
        if (airportJourney is AirportBookingRules.DropJourney or AirportBookingRules.RoundTripJourney
            && drop != AirportBookingRules.CanonicalLocation)
            throw new InvalidRequestException("Airport Drop and Round Trip must use Kempegowda International Airport (BLR) as the airport endpoint.");
        var notes = Optional(input.CustomerNotes, 1000, "Customer notes");
        var tripTypeId = TripType(requestedServiceType);
        var vehicleCode = Required(input.VehicleType, "Vehicle type", 32).ToLowerInvariant();
        var vehicle = await db.VehicleTypes.SingleOrDefaultAsync(x => x.Code == vehicleCode && x.IsActive, ct)
            ?? throw new InvalidRequestException("Choose a valid vehicle type.");
        var pickupLocal = ParsePickup(input.TravelDate, input.PickupTime);
        var returnLocal = ParseReturn(input.ReturnDate, input.ReturnTime, airportJourney, pickupLocal.Instant);
        var now = clock.GetUtcNow();
        if (pickupLocal.Instant <= now) throw new InvalidRequestException("Pickup date and time must be in the future.");
        if (pickupLocal.Instant > now.AddYears(2)) throw new InvalidRequestException("Pickup date is too far in the future.");

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            var year = pickupLocal.Date.Year;
            var number = await AllocateNumberAsync(year, ct);
            var booking = new Booking
            {
                Id = Guid.NewGuid(), BookingNumber = BookingNumberFormatter.Format(year, number), CustomerId = owner.Id,
                ContactName = owner.DisplayName, ContactMobileE164 = owner.User.PhoneE164!, ContactEmail = owner.User.Email,
                PickupAddress = pickup, DropAddress = drop, PickupAt = pickupLocal.Instant, PickupTimeZone = TimeZone,
                PickupLocalDate = pickupLocal.Date, RequestedVehicleTypeId = vehicle.Id, TripTypeId = tripTypeId,
                JourneyTypeId = airportJourney == AirportBookingRules.RoundTripJourney
                    ? ReferenceData.JourneyTypeRoundTrip : ReferenceData.JourneyTypeOneWay,
                ReturnAt = returnLocal?.Instant, ReturnLocalDate = returnLocal?.Date,
                StatusId = ReferenceData.BookingStatusPending,
                CustomerNotes = notes, IdempotencyKey = key
            };
            db.Bookings.Add(booking);
            db.BookingStatusHistories.Add(new BookingStatusHistory
            {
                BookingId = booking.Id, FromStatusId = null, ToStatusId = ReferenceData.BookingStatusPending,
                ChangedByUserId = userId, Reason = "Booking request received", CreatedAt = now
            });
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return await GetAsync(userId, booking.Id, ct);
        }
        catch (DbUpdateException ex) when (key is not null && IsUniqueViolation(ex))
        {
            await transaction.RollbackAsync(ct);
            db.ChangeTracker.Clear();
            var replay = await Query(owner.Id).SingleOrDefaultAsync(x => x.IdempotencyKey == key, ct);
            if (replay is not null) return Map(replay);
            throw;
        }
    }

    public async Task<IReadOnlyList<BookingResponse>> ListAsync(Guid userId, CancellationToken ct)
    {
        var customerId = await CustomerId(userId, ct);
        return (await Query(customerId).OrderByDescending(x => x.PickupAt).ThenByDescending(x => x.CreatedAt).ToListAsync(ct))
            .Select(Map).ToList();
    }

    public async Task<BookingResponse> GetAsync(Guid userId, Guid id, CancellationToken ct)
    {
        var customerId = await CustomerId(userId, ct);
        var booking = await Query(customerId).SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Booking was not found.");
        return Map(booking);
    }

    public async Task<BookingResponse> CancelAsync(Guid userId, Guid id, CancellationToken ct)
    {
        var customerId = await CustomerId(userId, ct);
        var booking = await db.Bookings.SingleOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId, ct)
            ?? throw new NotFoundException("Booking was not found.");
        if (!Cancellable.Contains(booking.StatusId))
            throw new ConflictException("This booking can no longer be cancelled online.");
        var from = booking.StatusId;
        booking.StatusId = ReferenceData.BookingStatusCancelled;
        db.BookingStatusHistories.Add(new BookingStatusHistory
        {
            BookingId = booking.Id, FromStatusId = from, ToStatusId = ReferenceData.BookingStatusCancelled,
            ChangedByUserId = userId, Reason = "Cancelled by customer", CreatedAt = clock.GetUtcNow()
        });
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { throw new ConflictException("The booking changed. Refresh and try again."); }
        return await GetAsync(userId, id, ct);
    }

    private IQueryable<Booking> Query(Guid customerId) => db.Bookings.AsNoTracking()
        .Include(x => x.Status).Include(x => x.TripType).Include(x => x.RequestedVehicleType)
        .Include(x => x.StatusHistory).ThenInclude(x => x.ToStatus)
        .Where(x => x.CustomerId == customerId);

    private async Task<Guid> CustomerId(Guid userId, CancellationToken ct) =>
        await db.Customers.Where(x => x.UserId == userId && x.StatusId == ReferenceData.CustomerStatusActive)
            .Select(x => x.Id).SingleOrDefaultAsync(ct) is var id && id != Guid.Empty
            ? id : throw new UnauthorizedException("An active customer account is required.");

    private async Task<long> AllocateNumberAsync(int year, CancellationToken ct)
    {
        var connection = (NpgsqlConnection)db.Database.GetDbConnection();
        await using var command = new NpgsqlCommand("""
            INSERT INTO booking_number_sequence (year, last_value) VALUES (@year, 1)
            ON CONFLICT (year) DO UPDATE SET last_value = booking_number_sequence.last_value + 1
            RETURNING last_value
            """, connection, (NpgsqlTransaction)db.Database.CurrentTransaction!.GetDbTransaction());
        command.Parameters.AddWithValue("year", year);
        return (long)(await command.ExecuteScalarAsync(ct))!;
    }

    private static (DateOnly Date, DateTimeOffset Instant) ParsePickup(string dateText, string timeText)
    {
        if (!DateOnly.TryParseExact(dateText, "yyyy-MM-dd", out var date) || !TimeOnly.TryParseExact(timeText, "HH:mm", out var time))
            throw new InvalidRequestException("Enter a valid travel date and pickup time.");
        var local = date.ToDateTime(time, DateTimeKind.Unspecified);
        var zone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        if (zone.IsInvalidTime(local)) throw new InvalidRequestException("Pickup time is invalid in the selected timezone.");
        return (date, new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(local, zone), TimeSpan.Zero));
    }

    private static short TripType(string value) => value.Trim().ToLowerInvariant() switch
    {
        AirportBookingRules.ServiceType => ReferenceData.TripTypeAirport,
        "local" => ReferenceData.TripTypeLocal,
        "outstation" => ReferenceData.TripTypeOutstation, "hourly" => ReferenceData.TripTypeCorporate,
        _ => throw new InvalidRequestException("Choose a valid service type.")
    };
    private static string? AirportJourney(string serviceType, string? value)
    {
        if (serviceType != AirportBookingRules.ServiceType)
        {
            if (!string.IsNullOrWhiteSpace(value)) throw new InvalidRequestException("Airport journey type is only valid for airport transfers.");
            return null;
        }
        return value?.Trim().ToLowerInvariant() switch
        {
            AirportBookingRules.PickupJourney => AirportBookingRules.PickupJourney,
            AirportBookingRules.DropJourney => AirportBookingRules.DropJourney,
            AirportBookingRules.RoundTripJourney => AirportBookingRules.RoundTripJourney,
            _ => throw new InvalidRequestException("Choose Pickup, Drop, or Round Trip for the airport transfer.")
        };
    }
    private static (DateOnly Date, DateTimeOffset Instant)? ParseReturn(string? date, string? time, string? journey, DateTimeOffset pickup)
    {
        if (journey != AirportBookingRules.RoundTripJourney)
        {
            if (!string.IsNullOrWhiteSpace(date) || !string.IsNullOrWhiteSpace(time))
                throw new InvalidRequestException("Return date and time are only valid for an airport round trip.");
            return null;
        }
        if (string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(time))
            throw new InvalidRequestException("Return date and time are required for an airport round trip.");
        var result = ParsePickup(date, time);
        if (result.Instant <= pickup) throw new InvalidRequestException("Return date and time must be after the outbound pickup.");
        return result;
    }
    private static string Required(string? value, string name, int max) => string.IsNullOrWhiteSpace(value)
        ? throw new InvalidRequestException($"{name} is required.")
        : value.Trim().Length > max ? throw new InvalidRequestException($"{name} is too long.") : value.Trim();
    private static string? Optional(string? value, int max, string name) => string.IsNullOrWhiteSpace(value) ? null : Required(value, name, max);
    private static string? NormalizeKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new InvalidRequestException("Idempotency-Key is required.");
        key = key.Trim();
        return key.Length is < 16 or > 64 ? throw new InvalidRequestException("Idempotency-Key must be 16 to 64 characters.") : key;
    }
    private static bool IsUniqueViolation(DbUpdateException ex) => ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    private static BookingResponse Map(Booking x) => new(x.Id, x.BookingNumber, x.PickupAddress, x.DropAddress ?? "",
        x.PickupAt, x.PickupTimeZone, x.PickupLocalDate, CustomerServiceType(x), CustomerAirportJourney(x),
        x.ReturnAt, x.ReturnLocalDate, x.RequestedVehicleType.Code,
        x.RequestedVehicleType.Name, x.Status.Code, Label(x.Status.Code), x.CustomerNotes, x.CreatedAt,
        Cancellable.Contains(x.StatusId), x.StatusHistory.OrderBy(h => h.CreatedAt)
            .Select(h => new BookingHistoryResponse(h.ToStatus.Code, Label(h.ToStatus.Code), h.CreatedAt, h.Reason)).ToList());
    private static string CustomerServiceType(Booking booking) => booking.TripTypeId == ReferenceData.TripTypeCorporate
        ? "hourly" : booking.TripType.Code;
    private static string? CustomerAirportJourney(Booking booking) => booking.TripTypeId != ReferenceData.TripTypeAirport
        ? null
        : booking.JourneyTypeId == ReferenceData.JourneyTypeRoundTrip
            ? AirportBookingRules.RoundTripJourney
            : booking.PickupAddress == AirportBookingRules.CanonicalLocation
                ? AirportBookingRules.PickupJourney : AirportBookingRules.DropJourney;
    public static string Label(string code) => code switch
    {
        "pending" => "Pending confirmation", "driver_assigned" => "Driver assigned", "driver_en_route" => "Driver en route",
        "picked_up" => "Picked up", _ => char.ToUpperInvariant(code[0]) + code[1..].Replace('_', ' ')
    };
}
