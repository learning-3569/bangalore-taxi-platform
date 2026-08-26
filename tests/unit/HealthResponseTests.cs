using BangaloreTaxi.Api.Health;

namespace BangaloreTaxi.UnitTests;

public sealed class HealthResponseTests
{
    [Fact]
    public void HealthResponse_preserves_phase_two_contract()
    {
        var utcNow = DateTimeOffset.Parse("2026-08-22T09:00:00Z");

        var response = new HealthResponse("ok", "BangaloreTaxi.Api", "2", utcNow);

        Assert.Equal("ok", response.Status);
        Assert.Equal("BangaloreTaxi.Api", response.Service);
        Assert.Equal("2", response.Phase);
        Assert.Equal(utcNow, response.UtcNow);
    }
}
