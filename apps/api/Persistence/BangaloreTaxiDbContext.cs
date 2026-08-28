using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BangaloreTaxi.Api.Persistence;

public sealed class BangaloreTaxiDbContext : DbContext
{
    public BangaloreTaxiDbContext(DbContextOptions<BangaloreTaxiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<DriverVehicleAssignment> DriverVehicleAssignments => Set<DriverVehicleAssignment>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();
    public DbSet<BookingNumberSequence> BookingNumberSequences => Set<BookingNumberSequence>();
    public DbSet<PricingPlan> PricingPlans => Set<PricingPlan>();
    public DbSet<PricingRate> PricingRates => Set<PricingRate>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SeoPage> SeoPages => Set<SeoPage>();
    public DbSet<SeoPageFaq> SeoPageFaqs => Set<SeoPageFaq>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<OtpChallenge> OtpChallenges => Set<OtpChallenge>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<OperationalSetting> OperationalSettings => Set<OperationalSetting>();

    public DbSet<UserStatus> UserStatuses => Set<UserStatus>();
    public DbSet<CustomerStatus> CustomerStatuses => Set<CustomerStatus>();
    public DbSet<DriverEmploymentStatus> DriverEmploymentStatuses => Set<DriverEmploymentStatus>();
    public DbSet<DriverAvailabilityStatus> DriverAvailabilityStatuses => Set<DriverAvailabilityStatus>();
    public DbSet<VehicleStatus> VehicleStatuses => Set<VehicleStatus>();
    public DbSet<BookingStatus> BookingStatuses => Set<BookingStatus>();
    public DbSet<TripType> TripTypes => Set<TripType>();
    public DbSet<JourneyType> JourneyTypes => Set<JourneyType>();
    public DbSet<PricingComponent> PricingComponents => Set<PricingComponent>();
    public DbSet<NotificationType> NotificationTypes => Set<NotificationType>();
    public DbSet<NotificationChannel> NotificationChannels => Set<NotificationChannel>();
    public DbSet<NotificationStatus> NotificationStatuses => Set<NotificationStatus>();
    public DbSet<SeoPageStatus> SeoPageStatuses => Set<SeoPageStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.HasPostgresExtension("btree_gist");
        modelBuilder.HasSequence<long>("driver_number_seq");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BangaloreTaxiDbContext).Assembly);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void StampTimestamps()
    {
        var utcNow = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries<IHasTimestamps>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAt == default)
                {
                    entry.Entity.CreatedAt = utcNow;
                }

                entry.Entity.UpdatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }
    }
}
