using BangaloreTaxi.Api.Persistence;

namespace BangaloreTaxi.UnitTests;

public sealed class BookingNumberFormatterTests
{
    [Fact]
    public void Format_matches_public_pattern()
    {
        Assert.Equal("BLR-2026-000001", BookingNumberFormatter.Format(2026, 1));
        Assert.Equal("BLR-2026-000042", BookingNumberFormatter.Format(2026, 42));
    }

    [Theory]
    [InlineData(1999, 1)]
    [InlineData(2026, 0)]
    [InlineData(2026, 1_000_000)]
    public void Format_rejects_out_of_range_parts(int year, long sequence)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BookingNumberFormatter.Format(year, sequence));
    }
}
