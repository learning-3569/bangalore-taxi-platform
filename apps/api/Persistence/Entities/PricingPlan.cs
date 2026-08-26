namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class PricingPlan : IHasTimestamps
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string CurrencyCode { get; set; }
    public DateTimeOffset EffectiveFrom { get; set; }
    public DateTimeOffset? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<PricingRate> Rates { get; set; } = new List<PricingRate>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
