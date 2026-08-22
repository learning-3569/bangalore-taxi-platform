using BangaloreTaxi.Api.Health;

namespace BangaloreTaxi.UnitTests;

public sealed class HealthResponseTests
{
    [Fact]
    public void HealthResponse_preserves_phase_zero_contract()
    {
        var utcNow = DateTimeOffset.Parse("2026-08-22T09:00:00Z");

        var response = new HealthResponse("ok", "BangaloreTaxi.Api", "0", utcNow);

        Assert.Equal("ok", response.Status);
        Assert.Equal("BangaloreTaxi.Api", response.Service);
        Assert.Equal("0", response.Phase);
        Assert.Equal(utcNow, response.UtcNow);
    }
}
