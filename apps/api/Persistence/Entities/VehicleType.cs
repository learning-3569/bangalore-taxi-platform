namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class VehicleType : IHasTimestamps
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public short TypicalCapacity { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public ICollection<Booking> RequestedOnBookings { get; set; } = new List<Booking>();
    public ICollection<PricingRate> PricingRates { get; set; } = new List<PricingRate>();
}
