using System.Security.Claims;
using BangaloreTaxi.Api.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BangaloreTaxi.Api.AdminBookings;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/v1/admin")]
public sealed class AdminFleetController(AdminFleetService fleet) : ControllerBase
{
    [HttpGet("drivers")]
    public Task<AdminDriverPage> Drivers([FromQuery] bool eligibleOnly = false, [FromQuery] string? search = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken cancellationToken = default) =>
        fleet.ListDriversAsync(eligibleOnly, search, page, pageSize, cancellationToken);

    [HttpGet("drivers/{id:guid}")]
    public Task<AdminDriverDetails> Driver(Guid id, CancellationToken cancellationToken) => fleet.GetDriverAsync(id, cancellationToken);

    [HttpPost("drivers"), EnableRateLimiting("admin-write")]
    public Task<AdminDriverDetails> CreateDriver([FromBody] CreateDriverRequest request, CancellationToken cancellationToken) =>
        fleet.CreateDriverAsync(UserId(), request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPut("drivers/{id:guid}"), EnableRateLimiting("admin-write")]
    public Task<AdminDriverDetails> UpdateDriver(Guid id, [FromBody] UpdateDriverRequest request, CancellationToken cancellationToken) =>
        fleet.UpdateDriverAsync(UserId(), id, request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPost("drivers/{id:guid}/deactivate"), EnableRateLimiting("admin-write")]
    public Task<AdminDriverDetails> DeactivateDriver(Guid id, [FromBody] FleetVersionRequest request, CancellationToken cancellationToken) => fleet.DeactivateDriverAsync(UserId(), id, request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPost("drivers/{id:guid}/reactivate"), EnableRateLimiting("admin-write")]
    public Task<AdminDriverDetails> ReactivateDriver(Guid id, [FromBody] FleetVersionRequest request, CancellationToken cancellationToken) => fleet.ReactivateDriverAsync(UserId(), id, request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPost("drivers/{id:guid}/vehicle"), EnableRateLimiting("admin-write")]
    public Task<AdminDriverDetails> TagVehicle(Guid id, [FromBody] TagVehicleRequest request, CancellationToken cancellationToken) => fleet.TagVehicleAsync(UserId(), id, request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpGet("vehicles")]
    public Task<AdminVehiclePage> Vehicles([FromQuery] bool eligibleOnly = false, [FromQuery] string? vehicleType = null,
        [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default) => fleet.ListVehiclesAsync(eligibleOnly, vehicleType, search, page, pageSize, cancellationToken);

    [HttpGet("vehicles/types")]
    public Task<IReadOnlyList<AdminVehicleTypeItem>> VehicleTypes(CancellationToken cancellationToken) => fleet.VehicleTypesAsync(cancellationToken);

    [HttpGet("vehicles/{id:guid}")]
    public Task<AdminVehicleDetails> Vehicle(Guid id, CancellationToken cancellationToken) => fleet.GetVehicleAsync(id, cancellationToken);

    [HttpPost("vehicles"), EnableRateLimiting("admin-write")]
    public Task<AdminVehicleDetails> CreateVehicle([FromBody] CreateVehicleRequest request, CancellationToken cancellationToken) => fleet.CreateVehicleAsync(UserId(), request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPut("vehicles/{id:guid}"), EnableRateLimiting("admin-write")]
    public Task<AdminVehicleDetails> UpdateVehicle(Guid id, [FromBody] UpdateVehicleRequest request, CancellationToken cancellationToken) => fleet.UpdateVehicleAsync(UserId(), id, request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPost("vehicles/{id:guid}/deactivate"), EnableRateLimiting("admin-write")]
    public Task<AdminVehicleDetails> DeactivateVehicle(Guid id, [FromBody] FleetVersionRequest request, CancellationToken cancellationToken) => fleet.DeactivateVehicleAsync(UserId(), id, request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPost("vehicles/{id:guid}/reactivate"), EnableRateLimiting("admin-write")]
    public Task<AdminVehicleDetails> ReactivateVehicle(Guid id, [FromBody] FleetVersionRequest request, CancellationToken cancellationToken) => fleet.ReactivateVehicleAsync(UserId(), id, request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    [HttpPost("vehicles/{id:guid}/driver"), EnableRateLimiting("admin-write")]
    public Task<AdminVehicleDetails> TagDriver(Guid id, [FromBody] TagDriverRequest request, CancellationToken cancellationToken) => fleet.TagDriverAsync(UserId(), id, request, HttpContext.Connection.RemoteIpAddress, cancellationToken);

    private Guid UserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : throw new UnauthorizedException("Session is no longer valid.");
    }
}
