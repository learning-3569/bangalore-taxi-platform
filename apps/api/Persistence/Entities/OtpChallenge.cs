namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class OtpChallenge
{
    public Guid Id { get; set; }
    public required string PhoneE164 { get; set; }
    public required string CodeHash { get; set; }
    public required string Salt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public short AttemptCount { get; set; }
    public DateTimeOffset? ConsumedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? RequestIp { get; set; }
}
