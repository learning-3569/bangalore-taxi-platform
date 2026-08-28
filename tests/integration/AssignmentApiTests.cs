using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BangaloreTaxi.Api.AdminBookings;
using BangaloreTaxi.Api.Auth;
using BangaloreTaxi.Api.Bookings;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BangaloreTaxi.IntegrationTests;

[Collection("postgres")]
public sealed class AssignmentApiTests(PostgresFixture postgres)
{
    [SkippableFact]
    public async Task Fleet_and_assignment_endpoints_are_admin_only_and_bounded()
    {
        RequireDatabase(); using var factory = Factory(); var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/api/v1/admin/drivers")).StatusCode);
        var (customer, _) = await Customer(factory); var booking = await Created(customer); var admin = await Admin(factory);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.GetAsync("/api/v1/admin/vehicles")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.PostAsJsonAsync($"/api/v1/admin/bookings/{booking.Id}/assignment", new { driverId = Guid.NewGuid(), vehicleId = Guid.NewGuid() })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.GetAsync("/api/v1/admin/drivers?page=0")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.GetAsync("/api/v1/admin/vehicles?pageSize=101")).StatusCode);
        var driver = await AddDriver(true); await AddDriver(false); var vehicle = await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, true); await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, false);
        var drivers = await admin.GetFromJsonAsync<AdminDriverPage>("/api/v1/admin/drivers?eligibleOnly=true&pageSize=100");
        var vehicles = await admin.GetFromJsonAsync<AdminVehiclePage>("/api/v1/admin/vehicles?eligibleOnly=true&vehicleType=sedan&pageSize=100");
        Assert.Contains(drivers!.Items, x => x.Id == driver); Assert.DoesNotContain(drivers.Items, x => !x.Eligible);
        Assert.Contains(vehicles!.Items, x => x.Id == vehicle); Assert.DoesNotContain(vehicles.Items, x => !x.Eligible || x.VehicleType != "sedan");
        var page = await admin.GetFromJsonAsync<AdminDriverPage>("/api/v1/admin/drivers?page=1&pageSize=1"); Assert.Single(page!.Items); Assert.Equal(1, page.PageSize);
    }

    [SkippableFact]
    public async Task Assignment_is_atomic_snapshotted_audited_and_customer_safe()
    {
        RequireDatabase(); using var factory = Factory(); var (customer, _) = await Customer(factory); var admin = await Admin(factory); var booking = await Accepted(customer, admin);
        var driver = await AddDriver(true, "Safe Driver"); var vehicle = await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, true);
        var response = await admin.PostAsJsonAsync($"/api/v1/admin/bookings/{booking.Id}/assignment", new { driverId = driver, vehicleId = vehicle }); response.EnsureSuccessStatusCode();
        var assigned = await response.Content.ReadFromJsonAsync<AdminBookingDetails>(); Assert.Equal("driver_assigned", assigned!.Status); Assert.False(assigned.CanAssign);
        Assert.Equal("Safe Driver", assigned.AssignedDriverName); Assert.Contains(assigned.History, x => x.Status == "driver_assigned");
        Assert.Equal(HttpStatusCode.Conflict, (await admin.PostAsJsonAsync($"/api/v1/admin/bookings/{booking.Id}/assignment", new { driverId = driver, vehicleId = vehicle })).StatusCode);
        var customerView = await customer.GetFromJsonAsync<BookingResponse>($"/api/v1/bookings/{booking.Id}"); Assert.False(customerView!.CanCancel); Assert.Equal("Safe Driver", customerView.AssignedDriverName);
        var json = await (await customer.GetAsync($"/api/v1/bookings/{booking.Id}")).Content.ReadAsStringAsync(); Assert.DoesNotContain("assignedDriverPhone", json, StringComparison.OrdinalIgnoreCase);
        var driverDetails = await admin.GetFromJsonAsync<AdminDriverDetails>($"/api/v1/admin/drivers/{driver}"); var vehicleDetails = await admin.GetFromJsonAsync<AdminVehicleDetails>($"/api/v1/admin/vehicles/{vehicle}");
        Assert.Equal(HttpStatusCode.Conflict, (await admin.PostAsJsonAsync($"/api/v1/admin/drivers/{driver}/deactivate", new { version = driverDetails!.Version })).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await admin.PostAsJsonAsync($"/api/v1/admin/vehicles/{vehicle}/deactivate", new { version = vehicleDetails!.Version })).StatusCode);
        await using var db = postgres.CreateContext(); var persisted = await db.Bookings.SingleAsync(x => x.Id == booking.Id);
        Assert.NotNull(persisted.EstimatedEndAt); Assert.NotNull(persisted.AssignmentWindow); Assert.Single(await db.AuditLogs.Where(x => x.EntityId == booking.Id && x.Action == "booking_assigned").ToListAsync());
    }

    [SkippableFact]
    public async Task Invalid_state_ineligible_resources_and_wrong_type_are_rejected()
    {
        RequireDatabase(); using var factory = Factory(); var (customer, _) = await Customer(factory); var admin = await Admin(factory); var pending = await Created(customer);
        var driver = await AddDriver(true); var sedan = await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, true);
        Assert.Equal(HttpStatusCode.Conflict, (await Assign(admin, pending.Id, driver, sedan)).StatusCode);
        var accepted = await Accepted(customer, admin, "Whitefield"); var badDriver = await AddDriver(false); var badVehicle = await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, false); var suv = await AddVehicle(ReferenceData.VehicleTypeIds.Suv, true);
        Assert.Equal(HttpStatusCode.Conflict, (await Assign(admin, accepted.Id, badDriver, sedan)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await Assign(admin, accepted.Id, driver, badVehicle)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await Assign(admin, accepted.Id, driver, suv)).StatusCode);
        var rejected = await Created(customer, "Rejected"); (await admin.PostAsJsonAsync($"/api/v1/admin/bookings/{rejected.Id}/reject", new { reason = "No capacity" })).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Conflict, (await Assign(admin, rejected.Id, driver, sedan)).StatusCode);
        var cancelled = await Accepted(customer, admin, "Cancelled"); (await customer.PostAsync($"/api/v1/bookings/{cancelled.Id}/cancel", null)).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Conflict, (await Assign(admin, cancelled.Id, driver, sedan)).StatusCode);
    }

    [SkippableFact]
    public async Task Overlap_and_concurrent_assignment_return_safe_conflicts()
    {
        RequireDatabase(); using var factory = Factory(); var (customer, _) = await Customer(factory); var admin = await Admin(factory);
        var first = await Accepted(customer, admin, "Overlap A"); var second = await Accepted(customer, admin, "Overlap B"); var third = await Accepted(customer, admin, "Overlap C");
        var d1 = await AddDriver(true); var d2 = await AddDriver(true); var v1 = await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, true); var v2 = await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, true);
        (await Assign(admin, first.Id, d1, v1)).EnsureSuccessStatusCode(); var conflict = await Assign(admin, second.Id, d1, v2); Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
        var body = await conflict.Content.ReadAsStringAsync(); Assert.DoesNotContain("constraint", body, StringComparison.OrdinalIgnoreCase); Assert.DoesNotContain("exclusion", body, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(HttpStatusCode.Conflict, (await Assign(admin, third.Id, d2, v1)).StatusCode);
        var competing = await Accepted(customer, admin, "Concurrent"); var d3 = await AddDriver(true); var d4 = await AddDriver(true); var v3 = await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, true); var v4 = await AddVehicle(ReferenceData.VehicleTypeIds.Sedan, true);
        var responses = await Task.WhenAll(Assign(admin, competing.Id, d3, v3), Assign(admin, competing.Id, d4, v4));
        Assert.Single(responses, x => x.IsSuccessStatusCode); Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Conflict);
    }

    private Task<HttpResponseMessage> Assign(HttpClient admin, Guid booking, Guid driver, Guid vehicle) => admin.PostAsJsonAsync($"/api/v1/admin/bookings/{booking}/assignment", new { driverId = driver, vehicleId = vehicle });
    private async Task<BookingResponse> Accepted(HttpClient customer, HttpClient admin, string pickup = "MG Road") { var booking = await Created(customer, pickup); (await admin.PostAsync($"/api/v1/admin/bookings/{booking.Id}/accept", null)).EnsureSuccessStatusCode(); return booking; }
    private async Task<Guid> AddDriver(bool eligible, string? name = null)
    {
        await using var db = postgres.CreateContext(); var now = DateTimeOffset.UtcNow; var userId = Guid.NewGuid(); var id = Guid.NewGuid();
        db.Users.Add(new User { Id = userId, PhoneE164 = "+918" + Random.Shared.NextInt64(100000000, 999999999), StatusId = ReferenceData.UserStatusActive, PhoneConfirmedAt = now, CreatedAt = now, UpdatedAt = now });
        db.Drivers.Add(new Driver { Id = id, UserId = userId, DisplayName = name ?? $"Driver {id:N}", EmploymentStatusId = eligible ? ReferenceData.DriverEmploymentActive : ReferenceData.DriverEmploymentInactive, AvailabilityStatusId = eligible ? ReferenceData.DriverAvailabilityAvailable : ReferenceData.DriverAvailabilityOffDuty, CreatedAt = now, UpdatedAt = now }); await db.SaveChangesAsync(); return id;
    }
    private async Task<Guid> AddVehicle(Guid type, bool eligible, string? registration = null)
    {
        await using var db = postgres.CreateContext(); var now = DateTimeOffset.UtcNow; var id = Guid.NewGuid();
        db.Vehicles.Add(new Vehicle { Id = id, RegistrationNumber = registration ?? $"KA{Random.Shared.Next(10, 99)}{Guid.NewGuid():N}"[..10].ToUpperInvariant(), VehicleTypeId = type, Capacity = 4, StatusId = eligible ? ReferenceData.VehicleStatusActive : ReferenceData.VehicleStatusMaintenance, CreatedAt = now, UpdatedAt = now }); await db.SaveChangesAsync(); return id;
    }
    private async Task<BookingResponse> Created(HttpClient client, string pickup = "MG Road")
    {
        var request = new { pickup, drop = AirportBookingRules.CanonicalLocation, serviceType = "airport", airportJourneyType = "drop", travelDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd"), pickupTime = "10:30", vehicleType = "sedan" };
        using var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/bookings") { Content = JsonContent.Create(request) }; message.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N")); var response = await client.SendAsync(message); response.EnsureSuccessStatusCode(); return (await response.Content.ReadFromJsonAsync<BookingResponse>())!;
    }
    private async Task<(HttpClient, AuthTokenResponse)> Customer(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient(); client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue); var phone = "+9195" + Random.Shared.Next(10000000, 99999999); await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var sender = factory.Services.GetRequiredService<DevelopmentPhoneOtpSender>(); Assert.True(sender.TryPeek(phone, out var otp)); var response = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp }); response.EnsureSuccessStatusCode(); var session = (await response.Content.ReadFromJsonAsync<AuthTokenResponse>())!; client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken); return (client, session);
    }
    private async Task<HttpClient> Admin(WebApplicationFactory<Program> factory)
    {
        var (client, session) = await Customer(factory); using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<BangaloreTaxiDbContext>(); db.UserRoles.Add(new UserRole { UserId = session.User.UserId, RoleId = ReferenceData.RoleIds.Admin }); await db.SaveChangesAsync(); var user = await db.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).Include(x => x.Customer).SingleAsync(x => x.Id == session.User.UserId); var token = scope.ServiceProvider.GetRequiredService<AccessTokenIssuer>().Issue(user, user.UserRoles.Select(x => x.Role.Code).ToList(), user.Customer?.Id).Token; client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); return client;
    }
    private WebApplicationFactory<Program> Factory() => new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { builder.UseEnvironment("Testing"); builder.UseSetting("ConnectionStrings:DefaultConnection", postgres.ConnectionString); builder.UseSetting("Auth:Otp:Pepper", "test-only-otp-pepper-change-me-32ch"); builder.UseSetting("Auth:Jwt:SigningKey", "test-only-jwt-signing-key-change-32"); builder.UseSetting("Auth:Otp:Provider", "Development"); });
    private void RequireDatabase() => Skip.If(!postgres.IsAvailable, "PostgreSQL is required.");
}
