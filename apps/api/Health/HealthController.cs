using Microsoft.AspNetCore.Mvc;

namespace BangaloreTaxi.Api.Health;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        return Ok(new HealthResponse(
            Status: "ok",
            Service: "BangaloreTaxi.Api",
            Phase: "0",
            UtcNow: DateTimeOffset.UtcNow));
    }
}

public sealed record HealthResponse(
    string Status,
    string Service,
    string Phase,
    DateTimeOffset UtcNow);
