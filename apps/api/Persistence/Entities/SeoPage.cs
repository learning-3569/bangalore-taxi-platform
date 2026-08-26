namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class SeoPage : IHasTimestamps
{
    public Guid Id { get; set; }
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public required string MetaDescription { get; set; }
    public required string H1 { get; set; }
    public required string Body { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public short StatusId { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public SeoPageStatus Status { get; set; } = null!;
    public ICollection<SeoPageFaq> Faqs { get; set; } = new List<SeoPageFaq>();
}
