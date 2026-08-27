using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BangaloreTaxi.Api.Auth;

[ApiController]
[Route("api/v1/auth")]
[EnableRateLimiting("auth")]
public sealed class AuthController : ControllerBase
{
    public const string BearerClientHeader = "X-Auth-Client";
    public const string BearerClientValue = "bearer";

    private readonly AuthService _auth;
    private readonly AuthCookieService _cookies;
    private readonly IHostEnvironment _environment;

    public AuthController(AuthService auth, AuthCookieService cookies, IHostEnvironment environment)
    {
        _auth = auth;
        _cookies = cookies;
        _environment = environment;
    }

    [HttpPost("otp/request")]
    [AllowAnonymous]
    public async Task<ActionResult<OtpRequestResponse>> RequestOtp([FromBody] OtpRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _auth.RequestOtpAsync(body.PhoneNumber, ClientIp(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("otp/verify")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthTokenResponse>> VerifyOtp([FromBody] OtpVerifyDto body, CancellationToken cancellationToken)
    {
        var bearer = WantsBearer();
        var session = await _auth.VerifyOtpAsync(
            body.PhoneNumber,
            body.Otp,
            ClientIp(),
            Request.Headers.UserAgent.ToString(),
            includeRefreshToken: bearer,
            cancellationToken);

        if (!bearer)
        {
            _cookies.SetSessionCookies(Response, session.RefreshTokenForCookie);
        }

        return Ok(ToResponse(session));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthTokenResponse>> Refresh([FromBody] RefreshRequestDto? body, CancellationToken cancellationToken)
    {
        var bearer = WantsBearer();
        if (!bearer && !_cookies.HasValidCsrf(Request))
        {
            return UnauthorizedProblem();
        }

        var token = bearer ? body?.RefreshToken : _cookies.ReadRefreshToken(Request) ?? body?.RefreshToken;
        var session = await _auth.RefreshAsync(
            token ?? "",
            ClientIp(),
            Request.Headers.UserAgent.ToString(),
            includeRefreshToken: bearer,
            cancellationToken);

        if (!bearer)
        {
            _cookies.SetSessionCookies(Response, session.RefreshTokenForCookie);
        }

        return Ok(ToResponse(session));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] RefreshRequestDto? body, CancellationToken cancellationToken)
    {
        var bearer = WantsBearer();
        if (!bearer && Request.Cookies.Count > 0 && !_cookies.HasValidCsrf(Request))
        {
            return UnauthorizedProblem();
        }

        var token = bearer ? body?.RefreshToken : _cookies.ReadRefreshToken(Request) ?? body?.RefreshToken;
        Guid? userId = User.Identity?.IsAuthenticated == true ? ReadUserId() : null;
        await _auth.LogoutAsync(token, userId, ClientIp(), cancellationToken);
        _cookies.ClearSessionCookies(Response);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthUserResponse>> Me(CancellationToken cancellationToken)
    {
        var userId = ReadUserId() ?? throw new Application.UnauthorizedException("Session is no longer valid.");
        return Ok(await _auth.GetMeAsync(userId, cancellationToken));
    }

    [HttpGet("otp/dev-peek")]
    [AllowAnonymous]
    public ActionResult<object> DevPeek([FromQuery] string phoneNumber)
    {
        if (_environment.IsProduction() || (!_environment.IsDevelopment() && !_environment.IsEnvironment("Testing")))
        {
            return NotFound();
        }

        var otp = _auth.PeekDevelopmentOtp(phoneNumber);
        if (otp is null)
        {
            return NotFound();
        }

        return Ok(new { otp });
    }

    private bool WantsBearer() =>
        string.Equals(Request.Headers[BearerClientHeader], BearerClientValue, StringComparison.OrdinalIgnoreCase);

    private string? ClientIp() => HttpContext.Connection.RemoteIpAddress?.ToString();

    private Guid? ReadUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private static AuthTokenResponse ToResponse(AuthSessionResult session) => new()
    {
        AccessToken = session.AccessToken,
        AccessTokenExpiresAt = session.AccessTokenExpiresAt,
        RefreshToken = session.RefreshTokenForBody,
        User = session.User
    };

    private ActionResult UnauthorizedProblem()
    {
        return Unauthorized(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = "Session is no longer valid.",
            Type = "https://httpstatuses.io/401"
        });
    }
}
