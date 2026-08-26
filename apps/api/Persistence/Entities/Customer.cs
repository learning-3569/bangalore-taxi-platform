namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class Customer : IHasTimestamps
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string DisplayName { get; set; }
    public short StatusId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public CustomerStatus Status { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
