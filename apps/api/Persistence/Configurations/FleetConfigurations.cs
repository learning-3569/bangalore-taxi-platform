using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BangaloreTaxi.Api.Persistence.Configurations;

internal sealed class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
{
    public void Configure(EntityTypeBuilder<VehicleType> builder)
    {
        builder.ToTable("vehicle_type");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasData(ReferenceData.VehicleTypes);
    }
}

internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicle");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RegistrationNumber).HasMaxLength(16).IsRequired();
        builder.HasIndex(x => x.RegistrationNumber).IsUnique();
        builder.HasOne(x => x.VehicleType)
            .WithMany(x => x.Vehicles)
            .HasForeignKey(x => x.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class DriverVehicleAssignmentConfiguration : IEntityTypeConfiguration<DriverVehicleAssignment>
{
    public void Configure(EntityTypeBuilder<DriverVehicleAssignment> builder)
    {
        builder.ToTable("driver_vehicle_assignment", table =>
        {
            table.HasCheckConstraint(
                "ck_driver_vehicle_assignment_range",
                "assigned_to IS NULL OR assigned_to > assigned_from");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DriverId)
            .IsUnique()
            .HasFilter("assigned_to IS NULL")
            .HasDatabaseName("ux_driver_vehicle_assignment_current_driver");
        builder.HasIndex(x => x.VehicleId)
            .IsUnique()
            .HasFilter("assigned_to IS NULL")
            .HasDatabaseName("ux_driver_vehicle_assignment_current_vehicle");
        builder.HasOne(x => x.Driver)
            .WithMany(x => x.VehicleAssignments)
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.DriverAssignments)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedByUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
