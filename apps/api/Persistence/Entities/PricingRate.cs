namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class PricingRate
{
    public Guid Id { get; set; }
    public Guid PricingPlanId { get; set; }
    public Guid VehicleTypeId { get; set; }
    public short TripTypeId { get; set; }
    public short JourneyTypeId { get; set; }
    public short ComponentId { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public PricingPlan PricingPlan { get; set; } = null!;
    public VehicleType VehicleType { get; set; } = null!;
    public TripType TripType { get; set; } = null!;
    public JourneyType JourneyType { get; set; } = null!;
    public PricingComponent Component { get; set; } = null!;
}
