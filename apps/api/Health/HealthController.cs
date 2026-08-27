using Microsoft.AspNetCore.Mvc;

namespace BangaloreTaxi.Api.Health;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    private readonly TimeProvider _timeProvider;

    public HealthController(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Service identity for operators. Database readiness is <c>GET /health/ready</c>.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        return Ok(new HealthResponse(
            Status: "ok",
            Service: "BangaloreTaxi.Api",
            Phase: "5",
            UtcNow: _timeProvider.GetUtcNow()));
    }
}

public sealed record HealthResponse(
    string Status,
    string Service,
    string Phase,
    DateTimeOffset UtcNow);
