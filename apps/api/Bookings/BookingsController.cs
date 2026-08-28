using System.Security.Claims;
using BangaloreTaxi.Api.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BangaloreTaxi.Api.Bookings;

[ApiController]
[Authorize(Roles = "customer")]
[Route("api/v1/bookings")]
public sealed class BookingsController(BookingService bookings) : ControllerBase
{
    [HttpPost, EnableRateLimiting("public-write")]
    public async Task<ActionResult<BookingResponse>> Create(
        [FromBody] CreateBookingRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var result = await bookings.CreateAsync(UserId(), request, idempotencyKey, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet]
    public Task<IReadOnlyList<BookingResponse>> List(CancellationToken cancellationToken) =>
        bookings.ListAsync(UserId(), cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<BookingResponse> Get(Guid id, CancellationToken cancellationToken) =>
        bookings.GetAsync(UserId(), id, cancellationToken);

    [HttpPost("{id:guid}/cancel"), EnableRateLimiting("public-write")]
    public Task<BookingResponse> Cancel(Guid id, CancellationToken cancellationToken) =>
        bookings.CancelAsync(UserId(), id, cancellationToken);

    private Guid UserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : throw new UnauthorizedException("Session is no longer valid.");
    }
}
