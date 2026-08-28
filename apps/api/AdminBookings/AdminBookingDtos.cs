using System.ComponentModel.DataAnnotations;

namespace BangaloreTaxi.Api.AdminBookings;

public sealed record AdminBookingListItem(
    Guid Id, string BookingNumber, string Status, string StatusLabel, string Pickup, string Drop,
    DateTimeOffset PickupAt, string PickupTimezone, DateOnly PickupLocalDate, string VehicleType,
    string VehicleTypeName, DateTimeOffset CreatedAt);

public sealed record AdminBookingPage(
    IReadOnlyList<AdminBookingListItem> Items, int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record AdminBookingHistory(
    string? FromStatus, string Status, string StatusLabel, DateTimeOffset CreatedAt, string? Reason);

public sealed record AdminBookingDetails(
    Guid Id, string BookingNumber, string Status, string StatusLabel, string ServiceType,
    string? AirportJourneyType, string Pickup, string Drop, DateTimeOffset PickupAt,
    string PickupTimezone, DateOnly PickupLocalDate, DateTimeOffset? ReturnAt, DateOnly? ReturnLocalDate,
    string VehicleType, string VehicleTypeName, string ContactName, string ContactMobile,
    string? ContactEmail, string? CustomerNotes, DateTimeOffset CreatedAt, bool CanAccept,
    bool CanReject, bool CanAssign, string? AssignedDriverName, string? AssignedVehicleRegistration,
    string? AssignedVehicleTypeName, IReadOnlyList<AdminBookingHistory> History);

public sealed class RejectBookingRequest
{
    [Required, StringLength(300, MinimumLength = 3)]
    public string Reason { get; init; } = "";
}

public sealed class AssignBookingRequest
{
    public Guid DriverId { get; init; }
    public Guid VehicleId { get; init; }
}
