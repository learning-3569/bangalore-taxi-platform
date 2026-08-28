using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BangaloreTaxi.Api.AdminBookings;
using BangaloreTaxi.Api.Auth;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BangaloreTaxi.IntegrationTests;

[Collection("postgres")]
public sealed class FleetManagementApiTests(PostgresFixture postgres)
{
    [SkippableFact]
    public async Task Fleet_writes_require_persisted_admin_role()
    {
        RequireDatabase(); using var factory = Factory(); var anonymous = factory.CreateClient(); var (customer, _) = await Customer(factory);
        var driver = DriverRequest("Auth Driver"); var vehicle = VehicleRequest();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.PostAsJsonAsync("/api/v1/admin/drivers", driver)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.PostAsJsonAsync("/api/v1/admin/drivers", driver)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.PostAsJsonAsync("/api/v1/admin/vehicles", vehicle)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.PutAsJsonAsync($"/api/v1/admin/drivers/{Guid.NewGuid()}", new { })).StatusCode);
    }

    [SkippableFact]
    public async Task Driver_creation_normalizes_phone_assigns_role_audits_and_generates_concurrent_numbers()
    {
        RequireDatabase(); using var factory = Factory(); var admin = await Admin(factory); var phoneDigits = "8" + Random.Shared.NextInt64(100000000, 999999999);
        var response = await admin.PostAsJsonAsync("/api/v1/admin/drivers", DriverRequest("Ramesh Kumar", phoneDigits)); response.EnsureSuccessStatusCode();
        var driver = (await response.Content.ReadFromJsonAsync<AdminDriverDetails>())!;
        Assert.Matches("^DRV-[0-9]{6}$", driver.DriverNumber); Assert.Equal("+91" + phoneDigits, driver.PhoneNumber); Assert.True(driver.Version > 0);
        Assert.Equal(HttpStatusCode.Conflict, (await admin.PostAsJsonAsync("/api/v1/admin/drivers", DriverRequest("Duplicate", phoneDigits))).StatusCode);

        var concurrent = await Task.WhenAll(
            admin.PostAsJsonAsync("/api/v1/admin/drivers", DriverRequest("Concurrent A")),
            admin.PostAsJsonAsync("/api/v1/admin/drivers", DriverRequest("Concurrent B")));
        Assert.All(concurrent, x => x.EnsureSuccessStatusCode());
        var numbers = await Task.WhenAll(concurrent.Select(x => x.Content.ReadFromJsonAsync<AdminDriverDetails>()));
        Assert.Equal(2, numbers.Select(x => x!.DriverNumber).Distinct().Count());

        await using var db = postgres.CreateContext(); var persisted = await db.Drivers.Include(x => x.User).ThenInclude(x => x.UserRoles).SingleAsync(x => x.Id == driver.Id);
        Assert.Contains(persisted.User.UserRoles, x => x.RoleId == ReferenceData.RoleIds.Driver);
        Assert.Single(await db.AuditLogs.Where(x => x.EntityId == driver.Id && x.Action == "driver_created").ToListAsync());
    }

    [SkippableFact]
    public async Task Driver_update_deactivate_reactivate_and_concurrency_preserve_identity()
    {
        RequireDatabase(); using var factory = Factory(); var admin = await Admin(factory); var driver = await CreateDriver(admin, "Editable Driver"); var number = driver.DriverNumber;
        var update = await admin.PutAsJsonAsync($"/api/v1/admin/drivers/{driver.Id}", new { displayName = "Edited Driver", phoneNumber = "9876543210", employmentStatus = "active", availabilityStatus = "off_duty", version = driver.Version }); update.EnsureSuccessStatusCode();
        var edited = (await update.Content.ReadFromJsonAsync<AdminDriverDetails>())!; Assert.Equal(number, edited.DriverNumber); Assert.Equal("+919876543210", edited.PhoneNumber); Assert.Equal("off_duty", edited.AvailabilityStatus);
        var stale = await admin.PutAsJsonAsync($"/api/v1/admin/drivers/{driver.Id}", new { displayName = "Stale", phoneNumber = edited.PhoneNumber, employmentStatus = "active", availabilityStatus = "available", version = driver.Version }); Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        var deactivate = await admin.PostAsJsonAsync($"/api/v1/admin/drivers/{driver.Id}/deactivate", new { version = edited.Version }); deactivate.EnsureSuccessStatusCode(); var inactive = (await deactivate.Content.ReadFromJsonAsync<AdminDriverDetails>())!; Assert.Equal("inactive", inactive.EmploymentStatus); Assert.False(inactive.Eligible);
        var eligible = await admin.GetFromJsonAsync<AdminDriverPage>("/api/v1/admin/drivers?eligibleOnly=true&pageSize=100"); Assert.DoesNotContain(eligible!.Items, x => x.Id == driver.Id);
        var reactivate = await admin.PostAsJsonAsync($"/api/v1/admin/drivers/{driver.Id}/reactivate", new { version = inactive.Version }); reactivate.EnsureSuccessStatusCode(); var active = (await reactivate.Content.ReadFromJsonAsync<AdminDriverDetails>())!; Assert.Equal("active", active.EmploymentStatus); Assert.Equal("off_duty", active.AvailabilityStatus);
        await using var db = postgres.CreateContext(); Assert.True(await db.Drivers.AnyAsync(x => x.Id == driver.Id)); Assert.Contains(await db.AuditLogs.Where(x => x.EntityId == driver.Id).Select(x => x.Action).ToListAsync(), x => x == "driver_deactivated"); Assert.Contains(await db.AuditLogs.Where(x => x.EntityId == driver.Id).Select(x => x.Action).ToListAsync(), x => x == "driver_reactivated");
    }

    [SkippableFact]
    public async Task Vehicle_crud_validates_duplicates_concurrency_and_soft_deactivation()
    {
        RequireDatabase(); using var factory = Factory(); var admin = await Admin(factory); var registration = UniqueRegistration();
        var create = await admin.PostAsJsonAsync("/api/v1/admin/vehicles", VehicleRequest(registration)); create.EnsureSuccessStatusCode(); var vehicle = (await create.Content.ReadFromJsonAsync<AdminVehicleDetails>())!;
        Assert.Equal(registration, vehicle.RegistrationNumber); Assert.Equal(HttpStatusCode.Conflict, (await admin.PostAsJsonAsync("/api/v1/admin/vehicles", VehicleRequest(registration))).StatusCode);
        var update = await admin.PutAsJsonAsync($"/api/v1/admin/vehicles/{vehicle.Id}", new { registrationNumber = registration + "X", vehicleTypeId = ReferenceData.VehicleTypeIds.Sedan, capacity = 5, status = "active", version = vehicle.Version }); update.EnsureSuccessStatusCode(); var edited = (await update.Content.ReadFromJsonAsync<AdminVehicleDetails>())!; Assert.Equal((short)5, edited.Capacity);
        Assert.Equal(HttpStatusCode.Conflict, (await admin.PutAsJsonAsync($"/api/v1/admin/vehicles/{vehicle.Id}", new { registrationNumber = registration, vehicleTypeId = ReferenceData.VehicleTypeIds.Sedan, capacity = 4, status = "active", version = vehicle.Version })).StatusCode);
        var deactivate = await admin.PostAsJsonAsync($"/api/v1/admin/vehicles/{vehicle.Id}/deactivate", new { version = edited.Version }); deactivate.EnsureSuccessStatusCode(); var inactive = (await deactivate.Content.ReadFromJsonAsync<AdminVehicleDetails>())!; Assert.Equal("inactive", inactive.Status); Assert.False(inactive.Eligible);
        var reactivate = await admin.PostAsJsonAsync($"/api/v1/admin/vehicles/{vehicle.Id}/reactivate", new { version = inactive.Version }); reactivate.EnsureSuccessStatusCode(); var active = (await reactivate.Content.ReadFromJsonAsync<AdminVehicleDetails>())!; Assert.Equal("active", active.Status);
        await using var db = postgres.CreateContext(); Assert.True(await db.Vehicles.AnyAsync(x => x.Id == vehicle.Id)); Assert.Single(await db.AuditLogs.Where(x => x.EntityId == vehicle.Id && x.Action == "vehicle_created").ToListAsync());
    }

    [SkippableFact]
    public async Task Roster_changes_are_transactional_historical_audited_and_one_to_one()
    {
        RequireDatabase(); using var factory = Factory(); var admin = await Admin(factory); var driver = await CreateDriver(admin, "Roster Driver"); var other = await CreateDriver(admin, "Other Driver"); var first = await CreateVehicle(admin); var second = await CreateVehicle(admin);
        var tagged = await admin.PostAsJsonAsync($"/api/v1/admin/drivers/{driver.Id}/vehicle", new { vehicleId = first.Id, version = driver.Version }); tagged.EnsureSuccessStatusCode(); var afterFirst = (await tagged.Content.ReadFromJsonAsync<AdminDriverDetails>())!; Assert.Equal(first.Id, afterFirst.CurrentVehicleId);
        var changed = await admin.PostAsJsonAsync($"/api/v1/admin/drivers/{driver.Id}/vehicle", new { vehicleId = second.Id, version = afterFirst.Version }); changed.EnsureSuccessStatusCode(); var afterSecond = (await changed.Content.ReadFromJsonAsync<AdminDriverDetails>())!; Assert.Equal(second.Id, afterSecond.CurrentVehicleId); Assert.Equal(2, afterSecond.VehicleHistory.Count); Assert.NotNull(afterSecond.VehicleHistory.Single(x => x.VehicleId == first.Id).AssignedTo);
        var currentVehicle = await admin.GetFromJsonAsync<AdminVehicleDetails>($"/api/v1/admin/vehicles/{second.Id}");
        var vehicleChange = await admin.PostAsJsonAsync($"/api/v1/admin/vehicles/{second.Id}/driver", new { driverId = other.Id, version = currentVehicle!.Version }); vehicleChange.EnsureSuccessStatusCode(); var vehicle = (await vehicleChange.Content.ReadFromJsonAsync<AdminVehicleDetails>())!; Assert.Equal(other.Id, vehicle.CurrentDriverId);
        var stale = await admin.PostAsJsonAsync($"/api/v1/admin/vehicles/{second.Id}/driver", new { driverId = driver.Id, version = currentVehicle.Version }); Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        await using var db = postgres.CreateContext(); Assert.Single(await db.DriverVehicleAssignments.Where(x => x.VehicleId == second.Id && x.AssignedTo == null).ToListAsync()); Assert.True(await db.DriverVehicleAssignments.CountAsync(x => x.VehicleId == second.Id) >= 2); Assert.Contains(await db.AuditLogs.Where(x => x.EntityType == "driver" && (x.EntityId == driver.Id || x.EntityId == other.Id)).Select(x => x.Action).ToListAsync(), x => x.Contains("driver_vehicle"));
    }

    private static object DriverRequest(string name, string? phone = null) => new { displayName = name, phoneNumber = phone ?? ("8" + Random.Shared.NextInt64(100000000, 999999999)), employmentStatus = "active", availabilityStatus = "available" };
    private static object VehicleRequest(string? registration = null) => new { registrationNumber = registration ?? UniqueRegistration(), vehicleTypeId = ReferenceData.VehicleTypeIds.Sedan, capacity = 4, status = "active" };
    private static string UniqueRegistration() => ("KA" + Guid.NewGuid().ToString("N"))[..12].ToUpperInvariant();
    private static async Task<AdminDriverDetails> CreateDriver(HttpClient admin, string name) { var response = await admin.PostAsJsonAsync("/api/v1/admin/drivers", DriverRequest(name)); response.EnsureSuccessStatusCode(); return (await response.Content.ReadFromJsonAsync<AdminDriverDetails>())!; }
    private static async Task<AdminVehicleDetails> CreateVehicle(HttpClient admin) { var response = await admin.PostAsJsonAsync("/api/v1/admin/vehicles", VehicleRequest()); response.EnsureSuccessStatusCode(); return (await response.Content.ReadFromJsonAsync<AdminVehicleDetails>())!; }
    private async Task<(HttpClient, AuthTokenResponse)> Customer(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient(); client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue); var phone = "+9194" + Random.Shared.Next(10000000, 99999999); await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone }); var sender = factory.Services.GetRequiredService<DevelopmentPhoneOtpSender>(); Assert.True(sender.TryPeek(phone, out var otp)); var response = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp }); response.EnsureSuccessStatusCode(); var session = (await response.Content.ReadFromJsonAsync<AuthTokenResponse>())!; client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken); return (client, session);
    }
    private async Task<HttpClient> Admin(WebApplicationFactory<Program> factory)
    {
        var (client, session) = await Customer(factory); using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<BangaloreTaxiDbContext>(); db.UserRoles.Add(new UserRole { UserId = session.User.UserId, RoleId = ReferenceData.RoleIds.Admin }); await db.SaveChangesAsync(); var user = await db.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).Include(x => x.Customer).SingleAsync(x => x.Id == session.User.UserId); var token = scope.ServiceProvider.GetRequiredService<AccessTokenIssuer>().Issue(user, user.UserRoles.Select(x => x.Role.Code).ToList(), user.Customer?.Id).Token; client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); return client;
    }
    private WebApplicationFactory<Program> Factory() => new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { builder.UseEnvironment("Testing"); builder.UseSetting("ConnectionStrings:DefaultConnection", postgres.ConnectionString); builder.UseSetting("Auth:Otp:Pepper", "test-only-otp-pepper-change-me-32ch"); builder.UseSetting("Auth:Jwt:SigningKey", "test-only-jwt-signing-key-change-32"); builder.UseSetting("Auth:Otp:Provider", "Development"); });
    private void RequireDatabase() => Skip.If(!postgres.IsAvailable, "PostgreSQL is required.");
}
