using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BangaloreTaxi.Api.Persistence.Configurations;

internal sealed class PricingPlanConfiguration : IEntityTypeConfiguration<PricingPlan>
{
    public void Configure(EntityTypeBuilder<PricingPlan> builder)
    {
        builder.ToTable("pricing_plan", table =>
        {
            table.HasCheckConstraint(
                "ck_pricing_plan_effective",
                "effective_to IS NULL OR effective_to > effective_from");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

internal sealed class PricingRateConfiguration : IEntityTypeConfiguration<PricingRate>
{
    public void Configure(EntityTypeBuilder<PricingRate> builder)
    {
        builder.ToTable("pricing_rate");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(12, 2);
        builder.HasIndex(x => new { x.PricingPlanId, x.VehicleTypeId, x.ComponentId, x.TripTypeId, x.JourneyTypeId })
            .IsUnique();
        builder.HasOne(x => x.PricingPlan)
            .WithMany(x => x.Rates)
            .HasForeignKey(x => x.PricingPlanId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.VehicleType)
            .WithMany(x => x.PricingRates)
            .HasForeignKey(x => x.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TripType).WithMany().HasForeignKey(x => x.TripTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.JourneyType).WithMany().HasForeignKey(x => x.JourneyTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Component).WithMany().HasForeignKey(x => x.ComponentId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notification");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityAlwaysColumn();
        builder.Property(x => x.FailureReason).HasMaxLength(500);
        builder.Property(x => x.ProviderMessageId).HasMaxLength(128);
        builder.HasIndex(x => new { x.BookingId, x.CreatedAt });
        builder.HasIndex(x => new { x.StatusId, x.CreatedAt })
            .HasFilter("status_id = 1")
            .HasDatabaseName("ix_notification_pending");
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RecipientUser)
            .WithMany()
            .HasForeignKey(x => x.RecipientUserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Type).WithMany().HasForeignKey(x => x.TypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Channel).WithMany().HasForeignKey(x => x.ChannelId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class SeoPageConfiguration : IEntityTypeConfiguration<SeoPage>
{
    public void Configure(EntityTypeBuilder<SeoPage> builder)
    {
        builder.ToTable("seo_page");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Slug).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(70).IsRequired();
        builder.Property(x => x.MetaDescription).HasMaxLength(320).IsRequired();
        builder.Property(x => x.H1).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CanonicalUrl).HasMaxLength(500);
        builder.Property(x => x.FeaturedImageUrl).HasMaxLength(500);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class SeoPageFaqConfiguration : IEntityTypeConfiguration<SeoPageFaq>
{
    public void Configure(EntityTypeBuilder<SeoPageFaq> builder)
    {
        builder.ToTable("seo_page_faq");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Question).HasMaxLength(300).IsRequired();
        builder.HasIndex(x => new { x.SeoPageId, x.SortOrder });
        builder.HasOne(x => x.SeoPage)
            .WithMany(x => x.Faqs)
            .HasForeignKey(x => x.SeoPageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_log");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityAlwaysColumn();
        builder.Property(x => x.Action).HasMaxLength(64).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(64).IsRequired();
        builder.Property(x => x.OldValue).HasColumnType("jsonb");
        builder.Property(x => x.NewValue).HasColumnType("jsonb");
        builder.Property(x => x.IpAddress).HasColumnType("inet");
        builder.HasIndex(x => new { x.EntityType, x.EntityId, x.CreatedAt }).IsDescending(false, false, true);
        builder.HasIndex(x => new { x.ActorUserId, x.CreatedAt })
            .IsDescending(false, true)
            .HasFilter("actor_user_id IS NOT NULL");
        builder.HasIndex(x => x.CreatedAt).IsDescending();
        builder.HasOne(x => x.ActorUser)
            .WithMany()
            .HasForeignKey(x => x.ActorUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class OperationalSettingConfiguration : IEntityTypeConfiguration<OperationalSetting>
{
    public void Configure(EntityTypeBuilder<OperationalSetting> builder)
    {
        builder.ToTable("operational_setting");
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).HasMaxLength(64);
        builder.Property(x => x.Value).HasMaxLength(64).IsRequired();
        builder.HasData(ReferenceData.OperationalSettings);
    }
}
