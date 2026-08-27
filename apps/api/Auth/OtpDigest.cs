using System.Security.Cryptography;
using System.Text;

namespace BangaloreTaxi.Api.Auth;

public static class OtpDigest
{
    public static string NewSalt() => Convert.ToHexString(RandomNumberGenerator.GetBytes(16));

    public static string Hash(string otp, string salt, string pepper)
    {
        var key = Encoding.UTF8.GetBytes(pepper);
        var data = Encoding.UTF8.GetBytes(salt + ":" + otp);
        var hash = HMACSHA256.HashData(key, data);
        return Convert.ToHexString(hash);
    }

    public static bool Equals(string otp, string salt, string pepper, string storedHash)
    {
        var actual = Hash(otp, salt, pepper);
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        var storedBytes = Encoding.UTF8.GetBytes(storedHash);
        return actualBytes.Length == storedBytes.Length
            && CryptographicOperations.FixedTimeEquals(actualBytes, storedBytes);
    }

    public static string NewOtp(int length)
    {
        if (length is < 4 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var max = (int)Math.Pow(10, length);
        var value = RandomNumberGenerator.GetInt32(0, max);
        return value.ToString(new string('0', length));
    }

    public static string HashRefreshToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

    public static string NewRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
