using BangaloreTaxi.Api.Auth;

namespace BangaloreTaxi.UnitTests;

public sealed class PhoneNormalizerTests
{
    [Theory]
    [InlineData("9876543210", "+919876543210")]
    [InlineData("+919876543210", "+919876543210")]
    [InlineData("919876543210", "+919876543210")]
    [InlineData("09876543210", "+919876543210")]
    [InlineData("98765 43210", "+919876543210")]
    public void Normalizes_indian_mobiles(string input, string expected)
    {
        Assert.True(PhoneNormalizer.TryNormalize(input, out var e164));
        Assert.Equal(expected, e164);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abcdefghij")]
    public void Rejects_invalid_numbers(string input)
    {
        Assert.False(PhoneNormalizer.TryNormalize(input, out _));
    }
}
