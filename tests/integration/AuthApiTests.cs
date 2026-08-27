using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BangaloreTaxi.Api.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BangaloreTaxi.IntegrationTests;

[Collection("postgres")]
public sealed class AuthApiTests
{
    private readonly PostgresFixture _postgres;
    private readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public AuthApiTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    [SkippableFact]
    public async Task Otp_flow_creates_customer_and_session()
    {
        RequireDatabase();
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue);

        var phone = UniquePhone();
        var request = await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone[3..] });
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        var body = await request.Content.ReadAsStringAsync();
        Assert.DoesNotContain("otp", body, StringComparison.OrdinalIgnoreCase);

        var otp = Peek(factory, phone);
        var verify = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp, role = "admin" });
        verify.EnsureSuccessStatusCode();
        var session = await verify.Content.ReadFromJsonAsync<AuthTokenResponse>(_json);
        Assert.NotNull(session);
        Assert.Contains("customer", session!.User.Roles);
        Assert.DoesNotContain("admin", session.User.Roles);
        Assert.False(string.IsNullOrWhiteSpace(session.RefreshToken));
        Assert.NotNull(session.User.CustomerId);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var me = await client.GetAsync("/api/v1/auth/me");
        me.EnsureSuccessStatusCode();
        var profile = await me.Content.ReadFromJsonAsync<AuthUserResponse>(_json);
        Assert.Equal(session.User.UserId, profile!.UserId);

        var refresh = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken = session.RefreshToken });
        refresh.EnsureSuccessStatusCode();
        var rotated = await refresh.Content.ReadFromJsonAsync<AuthTokenResponse>(_json);
        Assert.NotEqual(session.RefreshToken, rotated!.RefreshToken);

        var replay = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken = session.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);

        await client.PostAsJsonAsync("/api/v1/auth/logout", new { refreshToken = rotated.RefreshToken });
        var afterLogout = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken = rotated.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, afterLogout.StatusCode);
    }

    [SkippableFact]
    public async Task Invalid_phone_is_rejected_and_me_requires_auth()
    {
        RequireDatabase();
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        var bad = await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = "12" });
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);

        var me = await client.GetAsync("/api/v1/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, me.StatusCode);
    }

    [SkippableFact]
    public async Task Wrong_otp_and_reuse_are_rejected()
    {
        RequireDatabase();
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue);
        var phone = UniquePhone();
        await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var otp = Peek(factory, phone);

        var wrong = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp = "000000" });
        Assert.Equal(HttpStatusCode.Unauthorized, wrong.StatusCode);
        Assert.DoesNotContain("000000", await wrong.Content.ReadAsStringAsync(), StringComparison.Ordinal);

        var first = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp });
        first.EnsureSuccessStatusCode();
        var reuse = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp });
        Assert.Equal(HttpStatusCode.Unauthorized, reuse.StatusCode);
    }

    [SkippableFact]
    public async Task Existing_customer_is_reused()
    {
        RequireDatabase();
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue);
        var phone = UniquePhone();
        await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var first = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp = Peek(factory, phone) });
        var created = await first.Content.ReadFromJsonAsync<AuthTokenResponse>(_json);

        await Task.Delay(1100);
        await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var second = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp = Peek(factory, phone) });
        var again = await second.Content.ReadFromJsonAsync<AuthTokenResponse>(_json);
        Assert.Equal(created!.User.UserId, again!.User.UserId);
        Assert.Equal(created.User.CustomerId, again.User.CustomerId);
    }

    [SkippableFact]
    public async Task Logout_revokes_session_and_same_phone_can_verify_a_new_otp()
    {
        RequireDatabase();
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue);
        var phone = UniquePhone();
        await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var first = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp = Peek(factory, phone) });
        var session = await first.Content.ReadFromJsonAsync<AuthTokenResponse>(_json);
        await client.PostAsJsonAsync("/api/v1/auth/logout", new { refreshToken = session!.RefreshToken });
        var blocked = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken = session.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, blocked.StatusCode);

        await Task.Delay(1100);
        await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var second = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp = Peek(factory, phone) });
        second.EnsureSuccessStatusCode();
        var again = await second.Content.ReadFromJsonAsync<AuthTokenResponse>(_json);
        Assert.Equal(session.User.UserId, again!.User.UserId);
        Assert.NotEqual(session.RefreshToken, again.RefreshToken);
    }

    [SkippableFact]
    public async Task Resend_cooldown_and_attempt_limit_are_enforced()
    {
        RequireDatabase();
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue);
        var phone = UniquePhone();
        var first = await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        first.EnsureSuccessStatusCode();
        var second = await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        Assert.Equal(HttpStatusCode.TooManyRequests, second.StatusCode);
        Assert.NotNull(second.Headers.RetryAfter?.Delta);
        Assert.True(second.Headers.RetryAfter.Delta.Value.TotalSeconds >= 1);
        using var cooldownJson = await JsonDocument.ParseAsync(await second.Content.ReadAsStreamAsync());
        Assert.Equal("Please wait before requesting another code.", cooldownJson.RootElement.GetProperty("detail").GetString());
        Assert.True(cooldownJson.RootElement.GetProperty("retryAfterSeconds").GetInt32() >= 1);
        Assert.False(cooldownJson.RootElement.TryGetProperty("otpHash", out _));
        Assert.False(cooldownJson.RootElement.TryGetProperty("challengeId", out _));

        await Task.Delay(1100);
        await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        for (var i = 0; i < 5; i++)
        {
            var wrong = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp = "000000" });
            Assert.Equal(HttpStatusCode.Unauthorized, wrong.StatusCode);
        }

        var locked = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp = Peek(factory, phone) });
        Assert.Equal(HttpStatusCode.Unauthorized, locked.StatusCode);
    }

    [SkippableFact]
    public async Task Expired_otp_is_rejected()
    {
        RequireDatabase();
        using var factory = CreateFactory(expirySeconds: "1");
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(AuthController.BearerClientHeader, AuthController.BearerClientValue);
        var phone = UniquePhone();
        await client.PostAsJsonAsync("/api/v1/auth/otp/request", new { phoneNumber = phone });
        var otp = Peek(factory, phone);
        await Task.Delay(1200);
        var expired = await client.PostAsJsonAsync("/api/v1/auth/otp/verify", new { phoneNumber = phone, otp });
        Assert.Equal(HttpStatusCode.Unauthorized, expired.StatusCode);
    }

    private WebApplicationFactory<Program> CreateFactory(string? expirySeconds = null)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting("ConnectionStrings:DefaultConnection", _postgres.ConnectionString);
            builder.UseSetting("Auth:Otp:Pepper", "test-only-otp-pepper-change-me-32ch");
            builder.UseSetting("Auth:Jwt:SigningKey", "test-only-jwt-signing-key-change-32");
            builder.UseSetting("Auth:Otp:Provider", "Development");
            if (expirySeconds is not null)
            {
                builder.UseSetting("Auth:Otp:ExpirySeconds", expirySeconds);
            }
        });
    }

    private static string Peek(WebApplicationFactory<Program> factory, string phone)
    {
        var sender = factory.Services.GetRequiredService<DevelopmentPhoneOtpSender>();
        Assert.True(sender.TryPeek(phone, out var otp));
        return otp;
    }

    private static string UniquePhone()
    {
        return "+9198" + Random.Shared.Next(10000000, 99999999);
    }

    private void RequireDatabase()
    {
        Skip.If(!_postgres.IsAvailable, "PostgreSQL is required (Docker or SCHEMA_TEST_CONNECTION).");
    }
}
