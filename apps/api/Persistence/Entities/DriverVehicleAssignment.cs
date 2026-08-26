namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class DriverVehicleAssignment
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTimeOffset AssignedFrom { get; set; }
    public DateTimeOffset? AssignedTo { get; set; }
    public Guid? AssignedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Driver Driver { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
    public User? AssignedByUser { get; set; }
}
