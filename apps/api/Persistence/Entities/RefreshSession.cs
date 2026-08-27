namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class RefreshSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? ReplacedById { get; set; }
    public string? RequestIp { get; set; }
    public string? UserAgent { get; set; }

    public User User { get; set; } = null!;
    public RefreshSession? ReplacedBy { get; set; }
}
