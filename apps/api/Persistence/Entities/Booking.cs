using NpgsqlTypes;

namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class Booking : IHasTimestamps
{
    public Guid Id { get; set; }
    public required string BookingNumber { get; set; }
    public Guid? CustomerId { get; set; }
    public required string ContactName { get; set; }
    public required string ContactMobileE164 { get; set; }
    public string? ContactEmail { get; set; }
    public required string PickupAddress { get; set; }
    public decimal? PickupLatitude { get; set; }
    public decimal? PickupLongitude { get; set; }
    public string? DropAddress { get; set; }
    public decimal? DropLatitude { get; set; }
    public decimal? DropLongitude { get; set; }
    public DateTimeOffset PickupAt { get; set; }
    public required string PickupTimeZone { get; set; }
    public DateOnly PickupLocalDate { get; set; }
    public DateTimeOffset? EstimatedEndAt { get; set; }
    public decimal? EstimatedDistanceKm { get; set; }
    public decimal? EstimatedFareAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public Guid RequestedVehicleTypeId { get; set; }
    public short TripTypeId { get; set; }
    public short JourneyTypeId { get; set; }
    public Guid? AssignedDriverId { get; set; }
    public string? AssignedDriverDisplayName { get; set; }
    public string? AssignedDriverPhoneE164 { get; set; }
    public Guid? AssignedVehicleId { get; set; }
    public string? AssignedVehicleRegistration { get; set; }
    public string? AssignedVehicleTypeCode { get; set; }
    public string? AssignedVehicleTypeName { get; set; }
    public NpgsqlRange<DateTime>? AssignmentWindow { get; set; }
    public short StatusId { get; set; }
    public string? CustomerNotes { get; set; }
    public Guid? PricingPlanId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Customer? Customer { get; set; }
    public VehicleType RequestedVehicleType { get; set; } = null!;
    public TripType TripType { get; set; } = null!;
    public JourneyType JourneyType { get; set; } = null!;
    public Driver? AssignedDriver { get; set; }
    public Vehicle? AssignedVehicle { get; set; }
    public BookingStatus Status { get; set; } = null!;
    public PricingPlan? PricingPlan { get; set; }
    public ICollection<BookingStatusHistory> StatusHistory { get; set; } = new List<BookingStatusHistory>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
