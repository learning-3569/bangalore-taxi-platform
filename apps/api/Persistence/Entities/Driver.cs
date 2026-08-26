namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class Driver : IHasTimestamps
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string DisplayName { get; set; }
    public short EmploymentStatusId { get; set; }
    public short AvailabilityStatusId { get; set; }
    public string? LicenseNumber { get; set; }
    public DateOnly? LicenseExpiresOn { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public DriverEmploymentStatus EmploymentStatus { get; set; } = null!;
    public DriverAvailabilityStatus AvailabilityStatus { get; set; } = null!;
    public ICollection<DriverVehicleAssignment> VehicleAssignments { get; set; } = new List<DriverVehicleAssignment>();
    public ICollection<Booking> AssignedBookings { get; set; } = new List<Booking>();
}
