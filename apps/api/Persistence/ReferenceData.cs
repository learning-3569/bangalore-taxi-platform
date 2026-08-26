using BangaloreTaxi.Api.Persistence.Entities;

namespace BangaloreTaxi.Api.Persistence;

public static class ReferenceData
{
    public static readonly DateTimeOffset SeededAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static class RoleIds
    {
        public static readonly Guid Customer = Guid.Parse("a1111111-1111-4111-8111-000000000001");
        public static readonly Guid Admin = Guid.Parse("a1111111-1111-4111-8111-000000000002");
        public static readonly Guid Driver = Guid.Parse("a1111111-1111-4111-8111-000000000003");
    }

    public static class VehicleTypeIds
    {
        public static readonly Guid Sedan = Guid.Parse("b2222222-2222-4222-8222-000000000001");
        public static readonly Guid Suv = Guid.Parse("b2222222-2222-4222-8222-000000000002");
        public static readonly Guid Innova = Guid.Parse("b2222222-2222-4222-8222-000000000003");
        public static readonly Guid Premium = Guid.Parse("b2222222-2222-4222-8222-000000000004");
    }

    public const short UserStatusActive = 1;
    public const short UserStatusDisabled = 2;
    public const short UserStatusLocked = 3;

    public const short CustomerStatusActive = 1;
    public const short CustomerStatusInactive = 2;

    public const short DriverEmploymentActive = 1;
    public const short DriverEmploymentInactive = 2;
    public const short DriverEmploymentSuspended = 3;

    public const short DriverAvailabilityAvailable = 1;
    public const short DriverAvailabilityUnavailable = 2;
    public const short DriverAvailabilityOnTrip = 3;
    public const short DriverAvailabilityOffDuty = 4;

    public const short VehicleStatusActive = 1;
    public const short VehicleStatusInactive = 2;
    public const short VehicleStatusMaintenance = 3;

    public const short BookingStatusPending = 1;
    public const short BookingStatusAccepted = 2;
    public const short BookingStatusRejected = 3;
    public const short BookingStatusDriverAssigned = 4;
    public const short BookingStatusConfirmed = 5;
    public const short BookingStatusDriverEnRoute = 6;
    public const short BookingStatusPickedUp = 7;
    public const short BookingStatusCompleted = 8;
    public const short BookingStatusCancelled = 9;

    public const short TripTypeAirport = 1;
    public const short TripTypeLocal = 2;
    public const short TripTypeOutstation = 3;
    public const short TripTypeCorporate = 4;

    public const short JourneyTypeOneWay = 1;
    public const short JourneyTypeRoundTrip = 2;

    public const short SeoPageStatusDraft = 1;
    public const short SeoPageStatusPublished = 2;

    public const string AssignmentBufferMinutesKey = "assignment_buffer_minutes";
    public const string DefaultTripDurationMinutesKey = "default_trip_duration_minutes";

    public static Role[] Roles { get; } =
    [
        new() { Id = RoleIds.Customer, Code = "customer", Name = "Customer", CreatedAt = SeededAt },
        new() { Id = RoleIds.Admin, Code = "admin", Name = "Admin", CreatedAt = SeededAt },
        new() { Id = RoleIds.Driver, Code = "driver", Name = "Driver", CreatedAt = SeededAt }
    ];

    public static VehicleType[] VehicleTypes { get; } =
    [
        new()
        {
            Id = VehicleTypeIds.Sedan,
            Code = "sedan",
            Name = "Sedan",
            TypicalCapacity = 4,
            SortOrder = 1,
            IsActive = true,
            CreatedAt = SeededAt,
            UpdatedAt = SeededAt
        },
        new()
        {
            Id = VehicleTypeIds.Suv,
            Code = "suv",
            Name = "SUV",
            TypicalCapacity = 6,
            SortOrder = 2,
            IsActive = true,
            CreatedAt = SeededAt,
            UpdatedAt = SeededAt
        },
        new()
        {
            Id = VehicleTypeIds.Innova,
            Code = "innova",
            Name = "Innova",
            TypicalCapacity = 7,
            SortOrder = 3,
            IsActive = true,
            CreatedAt = SeededAt,
            UpdatedAt = SeededAt
        },
        new()
        {
            Id = VehicleTypeIds.Premium,
            Code = "premium",
            Name = "Premium",
            TypicalCapacity = 4,
            SortOrder = 4,
            IsActive = true,
            CreatedAt = SeededAt,
            UpdatedAt = SeededAt
        }
    ];

    public static OperationalSetting[] OperationalSettings { get; } =
    [
        new() { Key = AssignmentBufferMinutesKey, Value = "15", UpdatedAt = SeededAt },
        new() { Key = DefaultTripDurationMinutesKey, Value = "120", UpdatedAt = SeededAt }
    ];
}
