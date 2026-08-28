using System.ComponentModel.DataAnnotations;

namespace BangaloreTaxi.Api.Bookings;

public sealed class CreateBookingRequest
{
    [Required, StringLength(500, MinimumLength = 3)] public string Pickup { get; init; } = "";
    [Required, StringLength(500, MinimumLength = 3)] public string Drop { get; init; } = "";
    [Required, StringLength(32)] public string ServiceType { get; init; } = "";
    [StringLength(32)] public string? AirportJourneyType { get; init; }
    [Required] public string TravelDate { get; init; } = "";
    [Required] public string PickupTime { get; init; } = "";
    [Required, StringLength(32)] public string VehicleType { get; init; } = "";
    public string? ReturnDate { get; init; }
    public string? ReturnTime { get; init; }
    [StringLength(1000)] public string? CustomerNotes { get; init; }
}

public sealed record BookingHistoryResponse(string Status, string StatusLabel, DateTimeOffset CreatedAt, string? Reason);

public sealed record BookingResponse(
    Guid Id,
    string BookingNumber,
    string Pickup,
    string Drop,
    DateTimeOffset PickupAt,
    string PickupTimezone,
    DateOnly PickupLocalDate,
    string ServiceType,
    string? AirportJourneyType,
    DateTimeOffset? ReturnAt,
    DateOnly? ReturnLocalDate,
    string VehicleType,
    string VehicleTypeName,
    string Status,
    string StatusLabel,
    string? CustomerNotes,
    DateTimeOffset CreatedAt,
    bool CanCancel,
    IReadOnlyList<BookingHistoryResponse> History);
