using System.Text.RegularExpressions;

namespace BangaloreTaxi.Api.Auth;

/// <summary>
/// Normalizes user-entered phone numbers to E.164. Default region is India (+91).
/// Other country codes that already include '+' are accepted if they match E.164 shape.
/// </summary>
public static class PhoneNormalizer
{
    public const string IndiaCountryCode = "91";
    public const string E164Pattern = @"^\+[1-9][0-9]{7,14}$";

    private static readonly Regex E164 = new(E164Pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool TryNormalize(string? input, out string e164)
    {
        e164 = "";
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var digits = new string(input.Where(char.IsDigit).ToArray());
        if (digits.Length == 0)
        {
            return false;
        }

        string candidate;
        if (input.Trim().StartsWith('+'))
        {
            candidate = "+" + digits;
        }
        else if (digits.Length == 10 && digits[0] is >= '6' and <= '9')
        {
            candidate = "+" + IndiaCountryCode + digits;
        }
        else if (digits.Length == 11 && digits.StartsWith('0') && digits[1] is >= '6' and <= '9')
        {
            candidate = "+" + IndiaCountryCode + digits[1..];
        }
        else if (digits.Length == 12 && digits.StartsWith(IndiaCountryCode) && digits[2] is >= '6' and <= '9')
        {
            candidate = "+" + digits;
        }
        else if (digits.Length is >= 8 and <= 15)
        {
            candidate = "+" + digits;
        }
        else
        {
            return false;
        }

        if (!E164.IsMatch(candidate))
        {
            return false;
        }

        e164 = candidate;
        return true;
    }

    public static string LastFour(string e164)
    {
        return e164.Length >= 4 ? e164[^4..] : e164;
    }
}
