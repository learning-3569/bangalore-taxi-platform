using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BangaloreTaxi.Api.Configuration;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BangaloreTaxi.Api.Auth;

public sealed class AccessTokenIssuer
{
    private readonly AuthOptions _options;
    private readonly TimeProvider _clock;

    public AccessTokenIssuer(IOptions<AuthOptions> options, TimeProvider clock)
    {
        _options = options.Value;
        _clock = clock;
    }

    public (string Token, DateTimeOffset ExpiresAt) Issue(User user, IReadOnlyList<string> roles, Guid? customerId)
    {
        var expires = _clock.GetUtcNow().AddMinutes(_options.Jwt.AccessTokenMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Jwt.SigningKey));
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("phone", user.PhoneE164 ?? "")
        };
        if (customerId is Guid cid)
        {
            claims.Add(new Claim("customer_id", cid.ToString()));
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _options.Jwt.Issuer,
            audience: _options.Jwt.Audience,
            claims: claims,
            notBefore: _clock.GetUtcNow().UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
