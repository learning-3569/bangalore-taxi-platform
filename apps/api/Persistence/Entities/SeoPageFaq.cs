namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class SeoPageFaq
{
    public Guid Id { get; set; }
    public Guid SeoPageId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public SeoPage SeoPage { get; set; } = null!;
}
