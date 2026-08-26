namespace BangaloreTaxi.Api.Persistence.Entities;

public abstract class LookupEntity
{
    public short Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}

public sealed class UserStatus : LookupEntity;

public sealed class CustomerStatus : LookupEntity;

public sealed class DriverEmploymentStatus : LookupEntity;

public sealed class DriverAvailabilityStatus : LookupEntity;

public sealed class VehicleStatus : LookupEntity;

public sealed class BookingStatus : LookupEntity;

public sealed class TripType : LookupEntity;

public sealed class JourneyType : LookupEntity;

public sealed class PricingComponent : LookupEntity;

public sealed class NotificationType : LookupEntity;

public sealed class NotificationChannel : LookupEntity;

public sealed class NotificationStatus : LookupEntity;

public sealed class SeoPageStatus : LookupEntity;
