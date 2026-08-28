using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BangaloreTaxi.Api.Persistence.Configurations;

internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking", table =>
        {
            table.HasCheckConstraint(
                "ck_booking_pickup_coords",
                "(pickup_latitude IS NULL AND pickup_longitude IS NULL) OR (pickup_latitude IS NOT NULL AND pickup_longitude IS NOT NULL)");
            table.HasCheckConstraint(
                "ck_booking_drop_coords",
                "(drop_latitude IS NULL AND drop_longitude IS NULL) OR (drop_latitude IS NOT NULL AND drop_longitude IS NOT NULL)");
            table.HasCheckConstraint(
                "ck_booking_pickup_lat_range",
                "pickup_latitude IS NULL OR (pickup_latitude >= -90 AND pickup_latitude <= 90)");
            table.HasCheckConstraint(
                "ck_booking_pickup_lng_range",
                "pickup_longitude IS NULL OR (pickup_longitude >= -180 AND pickup_longitude <= 180)");
            table.HasCheckConstraint(
                "ck_booking_drop_lat_range",
                "drop_latitude IS NULL OR (drop_latitude >= -90 AND drop_latitude <= 90)");
            table.HasCheckConstraint(
                "ck_booking_drop_lng_range",
                "drop_longitude IS NULL OR (drop_longitude >= -180 AND drop_longitude <= 180)");
            table.HasCheckConstraint(
                "ck_booking_fare_currency",
                "estimated_fare_amount IS NULL OR currency_code IS NOT NULL");
            table.HasCheckConstraint(
                "ck_booking_return_complete",
                "(return_at IS NULL AND return_local_date IS NULL) OR (return_at IS NOT NULL AND return_local_date IS NOT NULL AND return_at > pickup_at)");
            table.HasCheckConstraint(
                "ck_booking_assignment_complete",
                "assigned_vehicle_id IS NULL OR (" +
                "assigned_driver_id IS NOT NULL AND estimated_end_at IS NOT NULL AND assignment_window IS NOT NULL " +
                "AND assigned_driver_display_name IS NOT NULL AND assigned_driver_phone_e164 IS NOT NULL " +
                "AND assigned_vehicle_registration IS NOT NULL AND assigned_vehicle_type_code IS NOT NULL " +
                "AND assigned_vehicle_type_name IS NOT NULL)");
        });

        builder.HasKey(x => x.Id);
        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsRowVersion();

        builder.Property(x => x.BookingNumber).HasMaxLength(24).IsRequired();
        builder.Property(x => x.ContactName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.ContactMobileE164).HasMaxLength(16).IsRequired();
        builder.Property(x => x.ContactEmail).HasColumnType("citext").HasMaxLength(256);
        builder.Property(x => x.PickupAddress).HasMaxLength(500).IsRequired();
        builder.Property(x => x.DropAddress).HasMaxLength(500);
        builder.Property(x => x.PickupLatitude).HasPrecision(9, 6);
        builder.Property(x => x.PickupLongitude).HasPrecision(9, 6);
        builder.Property(x => x.DropLatitude).HasPrecision(9, 6);
        builder.Property(x => x.DropLongitude).HasPrecision(9, 6);
        builder.Property(x => x.PickupTimeZone).HasMaxLength(64).IsRequired();
        builder.Property(x => x.EstimatedDistanceKm).HasPrecision(8, 2);
        builder.Property(x => x.EstimatedFareAmount).HasPrecision(12, 2);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.AssignedDriverDisplayName).HasMaxLength(120);
        builder.Property(x => x.AssignedDriverPhoneE164).HasMaxLength(16);
        builder.Property(x => x.AssignedVehicleRegistration).HasMaxLength(16);
        builder.Property(x => x.AssignedVehicleTypeCode).HasMaxLength(32);
        builder.Property(x => x.AssignedVehicleTypeName).HasMaxLength(64);
        builder.Property(x => x.AssignmentWindow).HasColumnType("tstzrange");
        builder.Property(x => x.CustomerNotes).HasMaxLength(1000);
        builder.Property(x => x.IdempotencyKey).HasMaxLength(64);

        builder.HasIndex(x => x.BookingNumber).IsUnique();
        builder.HasIndex(x => new { x.CustomerId, x.IdempotencyKey })
            .IsUnique()
            .HasFilter("customer_id IS NOT NULL AND idempotency_key IS NOT NULL");
        builder.HasIndex(x => new { x.ContactMobileE164, x.PickupAt }).IsDescending(false, true);
        builder.HasIndex(x => new { x.CustomerId, x.PickupAt })
            .IsDescending(false, true)
            .HasFilter("customer_id IS NOT NULL");
        builder.HasIndex(x => new { x.StatusId, x.PickupAt });
        builder.HasIndex(x => new { x.PickupLocalDate, x.StatusId });
        builder.HasIndex(x => x.PickupAt);
        builder.HasIndex(x => new { x.AssignedVehicleId, x.PickupAt })
            .HasFilter("assigned_vehicle_id IS NOT NULL");
        builder.HasIndex(x => new { x.AssignedDriverId, x.PickupAt })
            .HasFilter("assigned_driver_id IS NOT NULL");

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RequestedVehicleType)
            .WithMany(x => x.RequestedOnBookings)
            .HasForeignKey(x => x.RequestedVehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TripType).WithMany().HasForeignKey(x => x.TripTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.JourneyType).WithMany().HasForeignKey(x => x.JourneyTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedDriver)
            .WithMany(x => x.AssignedBookings)
            .HasForeignKey(x => x.AssignedDriverId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedVehicle)
            .WithMany(x => x.AssignedBookings)
            .HasForeignKey(x => x.AssignedVehicleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PricingPlan)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.PricingPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class BookingStatusHistoryConfiguration : IEntityTypeConfiguration<BookingStatusHistory>
{
    public void Configure(EntityTypeBuilder<BookingStatusHistory> builder)
    {
        builder.ToTable("booking_status_history");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityAlwaysColumn();
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.HasIndex(x => new { x.BookingId, x.CreatedAt });
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FromStatus)
            .WithMany()
            .HasForeignKey(x => x.FromStatusId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToStatus)
            .WithMany()
            .HasForeignKey(x => x.ToStatusId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ChangedByUser)
            .WithMany()
            .HasForeignKey(x => x.ChangedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class BookingNumberSequenceConfiguration : IEntityTypeConfiguration<BookingNumberSequence>
{
    public void Configure(EntityTypeBuilder<BookingNumberSequence> builder)
    {
        builder.ToTable("booking_number_sequence");
        builder.HasKey(x => x.Year);
        builder.Property(x => x.Year).ValueGeneratedNever();
    }
}
