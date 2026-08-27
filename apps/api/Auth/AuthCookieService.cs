using System.Security.Cryptography;
using System.Text;
using BangaloreTaxi.Api.Configuration;
using Microsoft.Extensions.Options;

namespace BangaloreTaxi.Api.Auth;

public sealed class AuthCookieService
{
    private readonly AuthOptions _options;
    private readonly IHostEnvironment _environment;

    public AuthCookieService(IOptions<AuthOptions> options, IHostEnvironment environment)
    {
        _options = options.Value;
        _environment = environment;
    }

    public string? ReadRefreshToken(HttpRequest request)
    {
        return request.Cookies.TryGetValue(_options.Cookie.RefreshName, out var value) ? value : null;
    }

    public bool HasValidCsrf(HttpRequest request)
    {
        if (!request.Cookies.TryGetValue(_options.Cookie.CsrfName, out var cookie) || string.IsNullOrEmpty(cookie))
        {
            return false;
        }

        var header = request.Headers["X-CSRF-Token"].ToString();
        if (string.IsNullOrEmpty(header))
        {
            return false;
        }

        var a = Encoding.UTF8.GetBytes(cookie);
        var b = Encoding.UTF8.GetBytes(header);
        return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
    }

    public void SetSessionCookies(HttpResponse response, string refreshToken)
    {
        var csrf = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        response.Cookies.Append(_options.Cookie.RefreshName, refreshToken, Build(httpOnly: true));
        response.Cookies.Append(_options.Cookie.CsrfName, csrf, Build(httpOnly: false));
    }

    public void ClearSessionCookies(HttpResponse response)
    {
        response.Cookies.Delete(_options.Cookie.RefreshName, Build(httpOnly: true));
        response.Cookies.Delete(_options.Cookie.CsrfName, Build(httpOnly: false));
    }

    private CookieOptions Build(bool httpOnly)
    {
        var sameSite = Enum.TryParse<SameSiteMode>(_options.Cookie.SameSite, ignoreCase: true, out var mode)
            ? mode
            : SameSiteMode.Lax;
        return new CookieOptions
        {
            HttpOnly = httpOnly,
            Secure = _environment.IsProduction() || _environment.IsEnvironment("Staging"),
            SameSite = sameSite,
            Path = _options.Cookie.Path,
            IsEssential = true
        };
    }
}
