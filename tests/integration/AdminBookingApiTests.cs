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
public sealed class AdminBookingApiTests(PostgresFixture postgres)
{
    [SkippableFact]
    public async Task Admin_endpoints_require_persisted_admin_role_and_ignore_role_spoofing()
    {
        RequireDatabase(); using var factory = Factory();
        var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/api/v1/admin/bookings")).StatusCode);

        var (customer, _) = await Customer(factory);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.GetAsync("/api/v1/admin/bookings")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.GetAsync($"/api/v1/admin/bookings/{Guid.NewGuid()}")).StatusCode);
        var spoof = new HttpRequestMessage(HttpMethod.Get, "/api/v1/admin/bookings");
        spoof.Headers.Add("X-Role", "admin");
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.SendAsync(spoof)).StatusCode);

        var created = await Created(customer);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.PostAsync($"/api/v1/admin/bookings/{created.Id}/accept", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.PostAsJsonAsync($"/api/v1/admin/bookings/{created.Id}/reject", new { reason = "spoof" })).StatusCode);

        var admin = await Admin(factory);
        (await admin.GetAsync("/api/v1/admin/bookings")).EnsureSuccessStatusCode();
    }

    [SkippableFact]
    public async Task Queue_is_filtered_paginated_deterministically_and_details_are_operational()
    {
        RequireDatabase(); using var factory = Factory(); var (customer, _) = await Customer(factory); var admin = await Admin(factory);
        var first = await Created(customer, "Admin queue A"); var second = await Created(customer, "Admin queue B");

        var page = await admin.GetFromJsonAsync<AdminBookingPage>("/api/v1/admin/bookings?status=pending&page=1&pageSize=1");
        Assert.NotNull(page); Assert.Single(page!.Items); Assert.True(page.TotalCount >= 2); Assert.True(page.TotalPages >= 2);
        Assert.Equal("pending", page.Items[0].Status);
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.GetAsync("/api/v1/admin/bookings?page=0")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.GetAsync("/api/v1/admin/bookings?pageSize=101")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.GetAsync("/api/v1/admin/bookings?status=made_up")).StatusCode);

        var details = await admin.GetFromJsonAsync<AdminBookingDetails>($"/api/v1/admin/bookings/{second.Id}");
        Assert.Equal(second.BookingNumber, details!.BookingNumber); Assert.Equal("pending", details.Status);
        Assert.False(string.IsNullOrWhiteSpace(details.ContactMobile)); Assert.True(details.CanAccept); Assert.True(details.CanReject);
        Assert.Single(details.History); Assert.Equal("airport", details.ServiceType);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"/api/v1/admin/bookings/{Guid.NewGuid()}")).StatusCode);
    }

    [SkippableFact]
    public async Task Accept_and_reject_are_atomic_audited_and_visible_to_the_customer()
    {
        RequireDatabase(); using var factory = Factory(); var (customer, _) = await Customer(factory); var admin = await Admin(factory);
        var accepted = await Created(customer, "Accept me");
        var acceptResponse = await admin.PostAsync($"/api/v1/admin/bookings/{accepted.Id}/accept", null); acceptResponse.EnsureSuccessStatusCode();
        var acceptedAdmin = await acceptResponse.Content.ReadFromJsonAsync<AdminBookingDetails>();
        Assert.Equal("accepted", acceptedAdmin!.Status); Assert.False(acceptedAdmin.CanAccept); Assert.Equal(2, acceptedAdmin.History.Count);
        var acceptedCustomer = await customer.GetFromJsonAsync<BookingResponse>($"/api/v1/bookings/{accepted.Id}");
        Assert.Equal("accepted", acceptedCustomer!.Status); Assert.Contains(acceptedCustomer.History, x => x.Reason == "Booking request accepted");
        Assert.Null(acceptedCustomer.CustomerNotes);
        Assert.Equal(HttpStatusCode.Conflict, (await admin.PostAsync($"/api/v1/admin/bookings/{accepted.Id}/accept", null)).StatusCode);

        var rejected = await Created(customer, "Reject me");
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.PostAsJsonAsync($"/api/v1/admin/bookings/{rejected.Id}/reject", new { reason = " " })).StatusCode);
        var rejectResponse = await admin.PostAsJsonAsync($"/api/v1/admin/bookings/{rejected.Id}/reject", new { reason = "Outside operating capacity" });
        rejectResponse.EnsureSuccessStatusCode();
        var rejectedCustomer = await customer.GetFromJsonAsync<BookingResponse>($"/api/v1/bookings/{rejected.Id}");
        Assert.Equal("rejected", rejectedCustomer!.Status); Assert.False(rejectedCustomer.CanCancel);
        Assert.Contains(rejectedCustomer.History, x => x.Reason == "Booking request not accepted");
        Assert.DoesNotContain(rejectedCustomer.History, x => x.Reason?.Contains("capacity", StringComparison.OrdinalIgnoreCase) == true);

        await using var db = postgres.CreateContext();
        Assert.Equal(2, await db.BookingStatusHistories.CountAsync(x => x.BookingId == accepted.Id));
        Assert.Equal(2, await db.BookingStatusHistories.CountAsync(x => x.BookingId == rejected.Id));
        Assert.Single(await db.AuditLogs.Where(x => x.EntityId == accepted.Id && x.Action == "booking_accepted").ToListAsync());
        var audit = await db.AuditLogs.SingleAsync(x => x.EntityId == rejected.Id && x.Action == "booking_rejected");
        Assert.Contains("Outside operating capacity", audit.NewValue);
    }

    [SkippableFact]
    public async Task Competing_admin_transitions_allow_exactly_one_winner()
    {
        RequireDatabase(); using var factory = Factory(); var (customer, _) = await Customer(factory); var admin = await Admin(factory);
        var booking = await Created(customer, "Concurrent decision");
        using var accept = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/admin/bookings/{booking.Id}/accept");
        using var reject = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/admin/bookings/{booking.Id}/reject") { Content = JsonContent.Create(new { reason = "Competing decision" }) };
        var responses = await Task.WhenAll(admin.SendAsync(accept), admin.SendAsync(reject));
        Assert.Single(responses, x => x.IsSuccessStatusCode);
        Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Conflict);
        await using var db = postgres.CreateContext();
        Assert.Equal(2, await db.BookingStatusHistories.CountAsync(x => x.BookingId == booking.Id));
        Assert.Single(await db.AuditLogs.Where(x => x.EntityId == booking.Id && (x.Action == "booking_accepted" || x.Action == "booking_rejected")).ToListAsync());
    }

    private async Task<BookingResponse> Created(HttpClient client, string pickup = "MG Road")
    {
        var local = DateTime.UtcNow.AddDays(3);
        var request = new { pickup, drop = AirportBookingRules.CanonicalLocation, serviceType = "airport", airportJourneyType = "drop", travelDate = local.ToString("yyyy-MM-dd"), pickupTime = "10:30", vehicleType = "sedan" };
        using var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/bookings") { Content = JsonContent.Create(request) };
        message.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        var response = await client.SendAsync(message); response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<BookingResponse>())!;
    }

    private async Task<(HttpClient Client, AuthTokenResponse Session)> Customer(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient(); client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue);
        var phone = "+9196" + Random.Shared.Next(10000000, 99999999); await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var sender = factory.Services.GetRequiredService<DevelopmentPhoneOtpSender>(); Assert.True(sender.TryPeek(phone, out var otp));
        var response = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp }); response.EnsureSuccessStatusCode();
        var session = (await response.Content.ReadFromJsonAsync<AuthTokenResponse>())!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        return (client, session);
    }

    private async Task<HttpClient> Admin(WebApplicationFactory<Program> factory)
    {
        var (client, session) = await Customer(factory);
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<BangaloreTaxiDbContext>();
        if (!await db.UserRoles.AnyAsync(x => x.UserId == session.User.UserId && x.RoleId == ReferenceData.RoleIds.Admin))
        {
            db.UserRoles.Add(new UserRole { UserId = session.User.UserId, RoleId = ReferenceData.RoleIds.Admin });
            await db.SaveChangesAsync();
        }
        var user = await db.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).Include(x => x.Customer).SingleAsync(x => x.Id == session.User.UserId);
        var issuer = scope.ServiceProvider.GetRequiredService<AccessTokenIssuer>();
        var token = issuer.Issue(user, user.UserRoles.Select(x => x.Role.Code).ToList(), user.Customer?.Id).Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private WebApplicationFactory<Program> Factory() => new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
    {
        builder.UseEnvironment("Testing"); builder.UseSetting("ConnectionStrings:DefaultConnection", postgres.ConnectionString);
        builder.UseSetting("Auth:Otp:Pepper", "test-only-otp-pepper-change-me-32ch");
        builder.UseSetting("Auth:Jwt:SigningKey", "test-only-jwt-signing-key-change-32"); builder.UseSetting("Auth:Otp:Provider", "Development");
    });
    private void RequireDatabase() => Skip.If(!postgres.IsAvailable, "PostgreSQL is required.");
}
