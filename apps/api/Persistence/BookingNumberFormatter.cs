namespace BangaloreTaxi.Api.Persistence;

public static class BookingNumberFormatter
{
    public const string Prefix = "BLR";

    public static string Format(int year, long sequence)
    {
        if (year is < 2000 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year), year, "Year must be four digits.");
        }

        if (sequence is < 1 or > 999_999)
        {
            throw new ArgumentOutOfRangeException(nameof(sequence), sequence, "Sequence must fit six digits.");
        }

        return $"{Prefix}-{year}-{sequence:D6}";
    }
}
