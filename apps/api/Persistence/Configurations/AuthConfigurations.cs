using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BangaloreTaxi.Api.Persistence.Configurations;

internal sealed class OtpChallengeConfiguration : IEntityTypeConfiguration<OtpChallenge>
{
    public void Configure(EntityTypeBuilder<OtpChallenge> builder)
    {
        builder.ToTable("otp_challenge");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PhoneE164).HasMaxLength(16).IsRequired();
        builder.Property(x => x.CodeHash).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Salt).HasMaxLength(32).IsRequired();
        builder.Property(x => x.RequestIp).HasMaxLength(64);
        builder.HasIndex(x => new { x.PhoneE164, x.CreatedAt }).IsDescending(false, true);
        builder.HasIndex(x => x.PhoneE164)
            .HasFilter("consumed_at IS NULL")
            .HasDatabaseName("ix_otp_challenge_phone_active");
    }
}

internal sealed class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        builder.ToTable("refresh_session");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
        builder.Property(x => x.RequestIp).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(256);
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.CreatedAt }).IsDescending(false, true);
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ReplacedBy)
            .WithMany()
            .HasForeignKey(x => x.ReplacedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
