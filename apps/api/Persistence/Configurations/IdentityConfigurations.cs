using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BangaloreTaxi.Api.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("role");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasData(ReferenceData.Roles);
    }
}

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", table =>
        {
            table.HasCheckConstraint("ck_users_email_or_phone", "email IS NOT NULL OR phone_e164 IS NOT NULL");
            table.HasCheckConstraint(
                "ck_users_phone_e164_format",
                "phone_e164 IS NULL OR phone_e164 ~ '^\\+[1-9][0-9]{7,14}$'");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasColumnType("citext").HasMaxLength(256);
        builder.Property(x => x.PhoneE164).HasMaxLength(16);
        builder.Property(x => x.PasswordHash).HasMaxLength(256);
        builder.HasIndex(x => x.Email).IsUnique().HasFilter("email IS NOT NULL");
        builder.HasIndex(x => x.PhoneE164).IsUnique().HasFilter("phone_e164 IS NOT NULL");
        builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_role");
        builder.HasKey(x => new { x.UserId, x.RoleId });
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedByUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customer");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasOne(x => x.User)
            .WithOne(x => x.Customer)
            .HasForeignKey<Customer>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("driver");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DriverNumber).HasMaxLength(10).IsRequired()
            .HasDefaultValueSql("'DRV-' || lpad(nextval('driver_number_seq')::text, 6, '0')")
            .ValueGeneratedOnAdd();
        builder.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.LicenseNumber).HasMaxLength(32);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.DriverNumber).IsUnique();
        builder.HasIndex(x => x.LicenseNumber).IsUnique().HasFilter("license_number IS NOT NULL");
        builder.HasIndex(x => new { x.EmploymentStatusId, x.AvailabilityStatusId });
        builder.HasOne(x => x.User)
            .WithOne(x => x.Driver)
            .HasForeignKey<Driver>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.EmploymentStatus)
            .WithMany()
            .HasForeignKey(x => x.EmploymentStatusId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AvailabilityStatus)
            .WithMany()
            .HasForeignKey(x => x.AvailabilityStatusId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property<uint>("xmin").HasColumnType("xid").ValueGeneratedOnAddOrUpdate().IsConcurrencyToken().IsRowVersion();
    }
}
