namespace BangaloreTaxi.Api.Configuration;

public sealed class RateLimitSettings
{
    public const string SectionName = "RateLimiting";

    /// <summary>Global fixed-window permits per client IP (foundation default).</summary>
    public int PermitLimit { get; set; } = 120;

    public int WindowSeconds { get; set; } = 60;
}
