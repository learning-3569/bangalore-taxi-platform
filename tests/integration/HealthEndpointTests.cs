using System.Net;
using System.Net.Http.Json;
using BangaloreTaxi.Api.Health;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BangaloreTaxi.IntegrationTests;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_api_health_returns_ok_payload()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(payload);
        Assert.Equal("ok", payload.Status);
        Assert.Equal("BangaloreTaxi.Api", payload.Service);
        Assert.Equal("5", payload.Phase);
    }
}
