using System.ComponentModel.DataAnnotations;

namespace BangaloreTaxi.Api.Auth;

public sealed class OtpRequestDto
{
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = "";
}

public sealed class OtpVerifyDto
{
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = "";

    [Required]
    [MaxLength(8)]
    public string Otp { get; set; } = "";
}

public sealed class RefreshRequestDto
{
    /// <summary>Used by mobile/bearer clients. Browser clients send the HttpOnly cookie instead.</summary>
    [MaxLength(128)]
    public string? RefreshToken { get; set; }
}

public sealed class OtpRequestResponse
{
    public bool Ok { get; init; } = true;
    public int ResendAvailableInSeconds { get; init; }
}

public sealed class AuthTokenResponse
{
    public required string AccessToken { get; init; }
    public required DateTimeOffset AccessTokenExpiresAt { get; init; }
    public string? RefreshToken { get; init; }
    public required AuthUserResponse User { get; init; }
}

public sealed class AuthUserResponse
{
    public required Guid UserId { get; init; }
    public Guid? CustomerId { get; init; }
    public required string PhoneNumber { get; init; }
    public required string MaskedPhone { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
}
