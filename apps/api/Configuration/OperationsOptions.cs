namespace BangaloreTaxi.Api.Configuration;

public sealed class OperationsOptions
{
    public const string SectionName = "Operations";

    public int AssignmentBufferMinutes { get; set; } = 15;

    public int DefaultTripDurationMinutes { get; set; } = 120;

    public string DefaultTimeZone { get; set; } = "Asia/Kolkata";

    public string DefaultCurrencyCode { get; set; } = "INR";
}
