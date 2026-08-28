using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BangaloreTaxi.Api.Auth;
using BangaloreTaxi.Api.Bookings;
using BangaloreTaxi.Api.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BangaloreTaxi.IntegrationTests;

[Collection("postgres")]
public sealed class BookingApiTests(PostgresFixture postgres)
{
    [SkippableFact]
    public async Task Creation_requires_auth_and_uses_authenticated_customer_with_pending_history()
    {
        RequireDatabase(); using var factory = Factory();
        var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await Post(anonymous, Request(), Key())).StatusCode);

        var (client, session) = await Customer(factory);
        var foreign = Guid.NewGuid();
        var request = Request() with { CustomerId = foreign, UserId = foreign, Role = "admin" };
        var response = await Post(client, request, Key()); response.EnsureSuccessStatusCode();
        var booking = await response.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.NotNull(booking); Assert.Matches(@"^BLR-\d{4}-\d{6}$", booking!.BookingNumber);
        Assert.Equal("pending", booking.Status); Assert.Equal("Pending confirmation", booking.StatusLabel);
        Assert.Single(booking.History); Assert.Equal("pending", booking.History[0].Status);

        await using var db = postgres.CreateContext();
        var row = await db.Bookings.SingleAsync(x => x.Id == booking.Id);
        Assert.Equal(session.User.CustomerId, row.CustomerId);
        Assert.Null(row.AssignedDriverId); Assert.Null(row.AssignedVehicleId); Assert.Null(row.PricingPlanId);
        Assert.Null(row.EstimatedFareAmount); Assert.Null(row.EstimatedDistanceKm); Assert.Null(row.EstimatedEndAt);
        Assert.Equal("Asia/Kolkata", row.PickupTimeZone); Assert.Equal(DateOnly.Parse(request.TravelDate), row.PickupLocalDate);
        Assert.Equal(TimeSpan.Zero, row.PickupAt.Offset);
    }

    [SkippableFact]
    public async Task Invalid_input_is_rejected_and_idempotency_replays_one_booking()
    {
        RequireDatabase(); using var factory = Factory(); var (client, session) = await Customer(factory);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, Request() with { Pickup = "" }, Key())).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, Request(), "short")).StatusCode);
        var key = Key(); var first = await Post(client, Request(), key); var second = await Post(client, Request(), key);
        first.EnsureSuccessStatusCode(); second.EnsureSuccessStatusCode();
        var a = await first.Content.ReadFromJsonAsync<BookingResponse>(); var b = await second.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.Equal(a!.Id, b!.Id); Assert.Equal(a.BookingNumber, b.BookingNumber);
        await using var db = postgres.CreateContext();
        Assert.Equal(1, await db.Bookings.CountAsync(x => x.CustomerId == session.User.CustomerId && x.IdempotencyKey == key));
    }

    [SkippableFact]
    public async Task Airport_direction_rejects_manipulation_and_persists_canonical_airport()
    {
        RequireDatabase(); using var factory = Factory(); var (client, _) = await Customer(factory);
        var pickupResponse = await Post(client, Request() with { AirportJourneyType = "pickup", Pickup = "Whitefield", Drop = "Mysore" }, Key());
        Assert.Equal(HttpStatusCode.BadRequest, pickupResponse.StatusCode);
        pickupResponse = await Post(client, Request() with { AirportJourneyType = "pickup", Pickup = AirportBookingRules.CanonicalLocation, Drop = "Mysore" }, Key());
        pickupResponse.EnsureSuccessStatusCode();
        var pickup = await pickupResponse.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.Equal(AirportBookingRules.CanonicalLocation, pickup!.Pickup);
        Assert.Equal("Mysore", pickup.Drop);
        Assert.Equal("airport", pickup.ServiceType); Assert.Equal("pickup", pickup.AirportJourneyType);

        var dropResponse = await Post(client, Request() with { AirportJourneyType = "drop", Pickup = "Whitefield", Drop = "Koramangala" }, Key());
        Assert.Equal(HttpStatusCode.BadRequest, dropResponse.StatusCode);
        dropResponse = await Post(client, Request() with { AirportJourneyType = "drop", Pickup = "Whitefield", Drop = AirportBookingRules.CanonicalLocation }, Key());
        dropResponse.EnsureSuccessStatusCode();
        var drop = await dropResponse.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.Equal("Whitefield", drop!.Pickup);
        Assert.Equal(AirportBookingRules.CanonicalLocation, drop.Drop);
        Assert.Equal("airport", drop.ServiceType); Assert.Equal("drop", drop.AirportJourneyType);

        var localResponse = await Post(client, Request() with { ServiceType = "local", AirportJourneyType = null, Pickup = "Whitefield", Drop = "Koramangala" }, Key());
        localResponse.EnsureSuccessStatusCode();
        var local = await localResponse.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.Equal("Whitefield", local!.Pickup); Assert.Equal("Koramangala", local.Drop); Assert.Equal("local", local.ServiceType);

        var outstationResponse = await Post(client, Request() with { ServiceType = "outstation", AirportJourneyType = null, Pickup = "Bengaluru", Drop = "Mysore" }, Key());
        outstationResponse.EnsureSuccessStatusCode();
        var outstation = await outstationResponse.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.Equal("Bengaluru", outstation!.Pickup); Assert.Equal("Mysore", outstation.Drop); Assert.Equal("outstation", outstation.ServiceType);

        var missingReturn = await Post(client, Request() with { AirportJourneyType = "round-trip", Drop = AirportBookingRules.CanonicalLocation }, Key());
        Assert.Equal(HttpStatusCode.BadRequest, missingReturn.StatusCode);
        var returnDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd");
        var roundTripResponse = await Post(client, Request() with { AirportJourneyType = "round-trip", Drop = AirportBookingRules.CanonicalLocation, ReturnDate = returnDate, ReturnTime = "12:00" }, Key());
        roundTripResponse.EnsureSuccessStatusCode();
        var roundTrip = await roundTripResponse.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.Equal(AirportBookingRules.CanonicalLocation, roundTrip!.Drop);
        Assert.Equal("round-trip", roundTrip.AirportJourneyType); Assert.NotNull(roundTrip.ReturnAt);

        await using var db = postgres.CreateContext();
        Assert.Equal(AirportBookingRules.CanonicalLocation, (await db.Bookings.SingleAsync(x => x.Id == pickup.Id)).PickupAddress);
        Assert.Equal(AirportBookingRules.CanonicalLocation, (await db.Bookings.SingleAsync(x => x.Id == drop.Id)).DropAddress);
    }

    [SkippableFact]
    public async Task Concurrent_creation_allocates_unique_sequentially_formatted_numbers()
    {
        RequireDatabase(); using var factory = Factory(); var (client, _) = await Customer(factory);
        var tasks = Enumerable.Range(0, 8).Select(i => Post(client, Request() with { Pickup = $"Pickup {i}" }, Key())).ToArray();
        var responses = await Task.WhenAll(tasks);
        foreach (var response in responses) response.EnsureSuccessStatusCode();
        var bookings = await Task.WhenAll(responses.Select(x => x.Content.ReadFromJsonAsync<BookingResponse>()));
        Assert.Equal(8, bookings.Select(x => x!.BookingNumber).Distinct().Count());
        Assert.All(bookings, x => Assert.Matches(@"^BLR-\d{4}-\d{6}$", x!.BookingNumber));
    }

    [SkippableFact]
    public async Task List_and_detail_enforce_customer_ownership()
    {
        RequireDatabase(); using var factory = Factory(); var (a, _) = await Customer(factory); var (b, _) = await Customer(factory);
        var createdA = await Created(a); var createdB = await Created(b);
        var list = await a.GetFromJsonAsync<List<BookingResponse>>("/api/v1/bookings");
        Assert.Contains(list!, x => x.Id == createdA.Id); Assert.DoesNotContain(list!, x => x.Id == createdB.Id);
        Assert.Equal(HttpStatusCode.NotFound, (await a.GetAsync($"/api/v1/bookings/{createdB.Id}")).StatusCode);
    }

    [SkippableFact]
    public async Task Cancellation_is_atomic_with_history_and_rejects_driver_assigned()
    {
        RequireDatabase(); using var factory = Factory(); var (client, _) = await Customer(factory);
        var allowed = await Created(client);
        var cancelledResponse = await client.PostAsync($"/api/v1/bookings/{allowed.Id}/cancel", null); cancelledResponse.EnsureSuccessStatusCode();
        var cancelled = await cancelledResponse.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.Equal("cancelled", cancelled!.Status); Assert.Equal(2, cancelled.History.Count); Assert.False(cancelled.CanCancel);

        var blocked = await Created(client);
        await using (var db = postgres.CreateContext()) { var row = await db.Bookings.SingleAsync(x => x.Id == blocked.Id); row.StatusId = ReferenceData.BookingStatusDriverAssigned; await db.SaveChangesAsync(); }
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsync($"/api/v1/bookings/{blocked.Id}/cancel", null)).StatusCode);
        await using var verify = postgres.CreateContext();
        Assert.Equal(ReferenceData.BookingStatusDriverAssigned, (await verify.Bookings.SingleAsync(x => x.Id == blocked.Id)).StatusId);
        Assert.Single(await verify.BookingStatusHistories.Where(x => x.BookingId == blocked.Id).ToListAsync());
    }

    private async Task<BookingResponse> Created(HttpClient client) { var response = await Post(client, Request(), Key()); response.EnsureSuccessStatusCode(); return (await response.Content.ReadFromJsonAsync<BookingResponse>())!; }
    private static Task<HttpResponseMessage> Post(HttpClient client, object request, string key) { var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/bookings") { Content = JsonContent.Create(request) }; message.Headers.Add("Idempotency-Key", key); return client.SendAsync(message); }
    private static BookingTestRequest Request() { var local = DateTime.UtcNow.AddDays(3); return new("MG Road", AirportBookingRules.CanonicalLocation, "airport", "drop", local.ToString("yyyy-MM-dd"), "10:30", "sedan", null, null, null, null, null); }
    private static string Key() => Guid.NewGuid().ToString("N");
    private async Task<(HttpClient Client, AuthTokenResponse Session)> Customer(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient(); client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue);
        var phone = "+9197" + Random.Shared.Next(10000000, 99999999); await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var sender = factory.Services.GetRequiredService<DevelopmentPhoneOtpSender>(); Assert.True(sender.TryPeek(phone, out var otp));
        var response = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp }); response.EnsureSuccessStatusCode();
        var session = (await response.Content.ReadFromJsonAsync<AuthTokenResponse>())!; client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken); return (client, session);
    }
    private WebApplicationFactory<Program> Factory() => new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { builder.UseEnvironment("Testing"); builder.UseSetting("ConnectionStrings:DefaultConnection", postgres.ConnectionString); builder.UseSetting("Auth:Otp:Pepper", "test-only-otp-pepper-change-me-32ch"); builder.UseSetting("Auth:Jwt:SigningKey", "test-only-jwt-signing-key-change-32"); builder.UseSetting("Auth:Otp:Provider", "Development"); });
    private void RequireDatabase() => Skip.If(!postgres.IsAvailable, "PostgreSQL is required.");
    private sealed record BookingTestRequest(string Pickup, string Drop, string ServiceType, string? AirportJourneyType, string TravelDate, string PickupTime, string VehicleType, string? ReturnDate, string? ReturnTime, Guid? CustomerId, Guid? UserId, string? Role);
}
