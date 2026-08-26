namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class OperationalSetting
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
