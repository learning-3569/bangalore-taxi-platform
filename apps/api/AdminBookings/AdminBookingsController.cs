using System.Security.Claims;
using BangaloreTaxi.Api.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BangaloreTaxi.Api.AdminBookings;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/v1/admin/bookings")]
public sealed class AdminBookingsController(AdminBookingService bookings) : ControllerBase
{
    [HttpGet]
    public Task<AdminBookingPage> List([FromQuery] string? status = "pending", [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25, CancellationToken cancellationToken = default) =>
        bookings.ListAsync(status, page, pageSize, cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<AdminBookingDetails> Get(Guid id, CancellationToken cancellationToken) =>
        bookings.GetAsync(id, cancellationToken);

    [HttpPost("{id:guid}/accept"), EnableRateLimiting("admin-write")]
    public Task<AdminBookingDetails> Accept(Guid id, CancellationToken cancellationToken) =>
        bookings.AcceptAsync(UserId(), id, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPost("{id:guid}/reject"), EnableRateLimiting("admin-write")]
    public Task<AdminBookingDetails> Reject(Guid id, [FromBody] RejectBookingRequest request, CancellationToken cancellationToken) =>
        bookings.RejectAsync(UserId(), id, request.Reason, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    private Guid UserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : throw new UnauthorizedException("Session is no longer valid.");
    }
}
