using System.ComponentModel.DataAnnotations;

namespace BangaloreTaxi.Api.AdminBookings;

public sealed record AdminDriverItem(
    Guid Id, string DriverNumber, string DisplayName, string PhoneNumber, string EmploymentStatus,
    string AvailabilityStatus, bool Eligible, Guid? CurrentVehicleId, string? CurrentVehicleRegistration);

public sealed record AdminDriverPage(
    IReadOnlyList<AdminDriverItem> Items, int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record AdminVehicleItem(
    Guid Id, string RegistrationNumber, string VehicleType, string VehicleTypeName, short Capacity,
    string Status, bool Eligible, Guid? CurrentDriverId, string? CurrentDriverNumber, string? CurrentDriverName);

public sealed record AdminVehiclePage(
    IReadOnlyList<AdminVehicleItem> Items, int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record AdminRosterHistory(
    Guid Id, Guid DriverId, string DriverNumber, string DriverName, Guid VehicleId,
    string VehicleRegistration, DateTimeOffset AssignedFrom, DateTimeOffset? AssignedTo);

public sealed record AdminDriverDetails(
    Guid Id, string DriverNumber, string DisplayName, string PhoneNumber, string EmploymentStatus,
    string AvailabilityStatus, bool Eligible, uint Version, Guid? CurrentVehicleId,
    string? CurrentVehicleRegistration, IReadOnlyList<AdminRosterHistory> VehicleHistory);

public sealed record AdminVehicleDetails(
    Guid Id, string RegistrationNumber, Guid VehicleTypeId, string VehicleType, string VehicleTypeName,
    short Capacity, string Status, bool Eligible, uint Version, Guid? CurrentDriverId,
    string? CurrentDriverNumber, string? CurrentDriverName, IReadOnlyList<AdminRosterHistory> DriverHistory);

public sealed record AdminVehicleTypeItem(Guid Id, string Code, string Name, short TypicalCapacity);

public class CreateDriverRequest
{
    [Required, StringLength(120, MinimumLength = 2)] public string DisplayName { get; init; } = "";
    [Required, StringLength(32)] public string PhoneNumber { get; init; } = "";
    [Required, StringLength(32)] public string EmploymentStatus { get; init; } = "active";
    [Required, StringLength(32)] public string AvailabilityStatus { get; init; } = "available";
}

public sealed class UpdateDriverRequest : CreateDriverRequest
{
    [Range(1, uint.MaxValue)] public uint Version { get; init; }
}

public class FleetVersionRequest
{
    [Range(1, uint.MaxValue)] public uint Version { get; init; }
}

public sealed class TagVehicleRequest : FleetVersionRequest
{
    public Guid? VehicleId { get; init; }
}

public class CreateVehicleRequest
{
    [Required, StringLength(16, MinimumLength = 4)] public string RegistrationNumber { get; init; } = "";
    public Guid VehicleTypeId { get; init; }
    [Range(1, 30)] public short Capacity { get; init; }
    [Required, StringLength(32)] public string Status { get; init; } = "active";
}

public sealed class UpdateVehicleRequest : CreateVehicleRequest
{
    [Range(1, uint.MaxValue)] public uint Version { get; init; }
}

public sealed class TagDriverRequest : FleetVersionRequest
{
    public Guid? DriverId { get; init; }
}
