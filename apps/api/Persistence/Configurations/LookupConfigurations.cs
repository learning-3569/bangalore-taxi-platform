using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BangaloreTaxi.Api.Persistence.Configurations;

internal static class LookupConfiguration
{
    public static void Configure<T>(EntityTypeBuilder<T> builder, string table, IEnumerable<T> seed)
        where T : LookupEntity
    {
        builder.ToTable(table);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasData(seed);
    }
}

internal sealed class UserStatusConfiguration : IEntityTypeConfiguration<UserStatus>
{
    public void Configure(EntityTypeBuilder<UserStatus> builder) =>
        LookupConfiguration.Configure(builder, "user_status",
        [
            new() { Id = ReferenceData.UserStatusActive, Code = "active", Name = "Active" },
            new() { Id = ReferenceData.UserStatusDisabled, Code = "disabled", Name = "Disabled" },
            new() { Id = ReferenceData.UserStatusLocked, Code = "locked", Name = "Locked" }
        ]);
}

internal sealed class CustomerStatusConfiguration : IEntityTypeConfiguration<CustomerStatus>
{
    public void Configure(EntityTypeBuilder<CustomerStatus> builder) =>
        LookupConfiguration.Configure(builder, "customer_status",
        [
            new() { Id = ReferenceData.CustomerStatusActive, Code = "active", Name = "Active" },
            new() { Id = ReferenceData.CustomerStatusInactive, Code = "inactive", Name = "Inactive" }
        ]);
}

internal sealed class DriverEmploymentStatusConfiguration : IEntityTypeConfiguration<DriverEmploymentStatus>
{
    public void Configure(EntityTypeBuilder<DriverEmploymentStatus> builder) =>
        LookupConfiguration.Configure(builder, "driver_employment_status",
        [
            new() { Id = ReferenceData.DriverEmploymentActive, Code = "active", Name = "Active" },
            new() { Id = ReferenceData.DriverEmploymentInactive, Code = "inactive", Name = "Inactive" },
            new() { Id = ReferenceData.DriverEmploymentSuspended, Code = "suspended", Name = "Suspended" }
        ]);
}

internal sealed class DriverAvailabilityStatusConfiguration : IEntityTypeConfiguration<DriverAvailabilityStatus>
{
    public void Configure(EntityTypeBuilder<DriverAvailabilityStatus> builder) =>
        LookupConfiguration.Configure(builder, "driver_availability_status",
        [
            new() { Id = ReferenceData.DriverAvailabilityAvailable, Code = "available", Name = "Available" },
            new() { Id = ReferenceData.DriverAvailabilityUnavailable, Code = "unavailable", Name = "Unavailable" },
            new() { Id = ReferenceData.DriverAvailabilityOnTrip, Code = "on_trip", Name = "On trip" },
            new() { Id = ReferenceData.DriverAvailabilityOffDuty, Code = "off_duty", Name = "Off duty" }
        ]);
}

internal sealed class VehicleStatusConfiguration : IEntityTypeConfiguration<VehicleStatus>
{
    public void Configure(EntityTypeBuilder<VehicleStatus> builder) =>
        LookupConfiguration.Configure(builder, "vehicle_status",
        [
            new() { Id = ReferenceData.VehicleStatusActive, Code = "active", Name = "Active" },
            new() { Id = ReferenceData.VehicleStatusInactive, Code = "inactive", Name = "Inactive" },
            new() { Id = ReferenceData.VehicleStatusMaintenance, Code = "maintenance", Name = "Maintenance" }
        ]);
}

internal sealed class BookingStatusConfiguration : IEntityTypeConfiguration<BookingStatus>
{
    public void Configure(EntityTypeBuilder<BookingStatus> builder) =>
        LookupConfiguration.Configure(builder, "booking_status",
        [
            new() { Id = ReferenceData.BookingStatusPending, Code = "pending", Name = "Pending" },
            new() { Id = ReferenceData.BookingStatusAccepted, Code = "accepted", Name = "Accepted" },
            new() { Id = ReferenceData.BookingStatusRejected, Code = "rejected", Name = "Rejected" },
            new() { Id = ReferenceData.BookingStatusDriverAssigned, Code = "driver_assigned", Name = "Driver assigned" },
            new() { Id = ReferenceData.BookingStatusConfirmed, Code = "confirmed", Name = "Confirmed" },
            new() { Id = ReferenceData.BookingStatusDriverEnRoute, Code = "driver_en_route", Name = "Driver en route" },
            new() { Id = ReferenceData.BookingStatusPickedUp, Code = "picked_up", Name = "Picked up" },
            new() { Id = ReferenceData.BookingStatusCompleted, Code = "completed", Name = "Completed" },
            new() { Id = ReferenceData.BookingStatusCancelled, Code = "cancelled", Name = "Cancelled" }
        ]);
}

internal sealed class TripTypeConfiguration : IEntityTypeConfiguration<TripType>
{
    public void Configure(EntityTypeBuilder<TripType> builder) =>
        LookupConfiguration.Configure(builder, "trip_type",
        [
            new() { Id = ReferenceData.TripTypeAirport, Code = "airport", Name = "Airport" },
            new() { Id = ReferenceData.TripTypeLocal, Code = "local", Name = "Local" },
            new() { Id = ReferenceData.TripTypeOutstation, Code = "outstation", Name = "Outstation" },
            new() { Id = ReferenceData.TripTypeCorporate, Code = "corporate", Name = "Corporate" }
        ]);
}

internal sealed class JourneyTypeConfiguration : IEntityTypeConfiguration<JourneyType>
{
    public void Configure(EntityTypeBuilder<JourneyType> builder) =>
        LookupConfiguration.Configure(builder, "journey_type",
        [
            new() { Id = ReferenceData.JourneyTypeOneWay, Code = "one_way", Name = "One way" },
            new() { Id = ReferenceData.JourneyTypeRoundTrip, Code = "round_trip", Name = "Round trip" }
        ]);
}

internal sealed class PricingComponentConfiguration : IEntityTypeConfiguration<PricingComponent>
{
    public void Configure(EntityTypeBuilder<PricingComponent> builder) =>
        LookupConfiguration.Configure(builder, "pricing_component",
        [
            new() { Id = 1, Code = "base_fare", Name = "Base fare" },
            new() { Id = 2, Code = "per_km", Name = "Per kilometre" },
            new() { Id = 3, Code = "minimum_fare", Name = "Minimum fare" },
            new() { Id = 4, Code = "airport_surcharge", Name = "Airport surcharge" },
            new() { Id = 5, Code = "night_surcharge", Name = "Night surcharge" },
            new() { Id = 6, Code = "waiting_per_minute", Name = "Waiting per minute" },
            new() { Id = 7, Code = "toll_pass_through", Name = "Toll pass-through" },
            new() { Id = 8, Code = "outstation_per_km", Name = "Outstation per kilometre" },
            new() { Id = 9, Code = "round_trip_multiplier", Name = "Round-trip multiplier" }
        ]);
}

internal sealed class NotificationTypeConfiguration : IEntityTypeConfiguration<NotificationType>
{
    public void Configure(EntityTypeBuilder<NotificationType> builder) =>
        LookupConfiguration.Configure(builder, "notification_type",
        [
            new() { Id = 1, Code = "booking_received", Name = "Booking received" },
            new() { Id = 2, Code = "booking_accepted", Name = "Booking accepted" },
            new() { Id = 3, Code = "booking_rejected", Name = "Booking rejected" },
            new() { Id = 4, Code = "booking_confirmed", Name = "Booking confirmed" },
            new() { Id = 5, Code = "driver_assigned", Name = "Driver assigned" },
            new() { Id = 6, Code = "trip_reminder", Name = "Trip reminder" },
            new() { Id = 7, Code = "booking_cancelled", Name = "Booking cancelled" },
            new() { Id = 8, Code = "admin_new_request", Name = "Admin new request" }
        ]);
}

internal sealed class NotificationChannelConfiguration : IEntityTypeConfiguration<NotificationChannel>
{
    public void Configure(EntityTypeBuilder<NotificationChannel> builder) =>
        LookupConfiguration.Configure(builder, "notification_channel",
        [
            new() { Id = 1, Code = "whatsapp", Name = "WhatsApp" },
            new() { Id = 2, Code = "sms", Name = "SMS" },
            new() { Id = 3, Code = "email", Name = "Email" }
        ]);
}

internal sealed class NotificationStatusConfiguration : IEntityTypeConfiguration<NotificationStatus>
{
    public void Configure(EntityTypeBuilder<NotificationStatus> builder) =>
        LookupConfiguration.Configure(builder, "notification_status",
        [
            new() { Id = 1, Code = "pending", Name = "Pending" },
            new() { Id = 2, Code = "sent", Name = "Sent" },
            new() { Id = 3, Code = "failed", Name = "Failed" }
        ]);
}

internal sealed class SeoPageStatusConfiguration : IEntityTypeConfiguration<SeoPageStatus>
{
    public void Configure(EntityTypeBuilder<SeoPageStatus> builder) =>
        LookupConfiguration.Configure(builder, "seo_page_status",
        [
            new() { Id = ReferenceData.SeoPageStatusDraft, Code = "draft", Name = "Draft" },
            new() { Id = ReferenceData.SeoPageStatusPublished, Code = "published", Name = "Published" }
        ]);
}
