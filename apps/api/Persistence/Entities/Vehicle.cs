namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class Vehicle : IHasTimestamps
{
    public Guid Id { get; set; }
    public required string RegistrationNumber { get; set; }
    public Guid VehicleTypeId { get; set; }
    public short Capacity { get; set; }
    public short StatusId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public VehicleType VehicleType { get; set; } = null!;
    public VehicleStatus Status { get; set; } = null!;
    public ICollection<DriverVehicleAssignment> DriverAssignments { get; set; } = new List<DriverVehicleAssignment>();
    public ICollection<Booking> AssignedBookings { get; set; } = new List<Booking>();
}
