namespace BangaloreTaxi.Api.Configuration;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public OtpOptions Otp { get; set; } = new();
    public JwtOptions Jwt { get; set; } = new();
    public AuthCookieSettings Cookie { get; set; } = new();
    public int RefreshTokenDays { get; set; } = 14;
}

public sealed class OtpOptions
{
    public int Length { get; set; } = 6;
    public int ExpirySeconds { get; set; } = 300;
    public int MaxAttempts { get; set; } = 5;
    public int ResendCooldownSeconds { get; set; } = 60;
    public int MaxRequestsPerHour { get; set; } = 5;
    public string Pepper { get; set; } = "";
    /// <summary>Development | Unconfigured. Production must not be Development.</summary>
    public string Provider { get; set; } = "Unconfigured";
}

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "bangalore-taxi-api";
    public string Audience { get; set; } = "bangalore-taxi-clients";
    public string SigningKey { get; set; } = "";
    public int AccessTokenMinutes { get; set; } = 15;
}

public sealed class AuthCookieSettings
{
    public string RefreshName { get; set; } = "bt_refresh";
    public string CsrfName { get; set; } = "bt_csrf";
    public string Path { get; set; } = "/api/v1/auth";
    public string SameSite { get; set; } = "Lax";
}
