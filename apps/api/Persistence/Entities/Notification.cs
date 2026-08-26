namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class Notification
{
    public long Id { get; set; }
    public Guid? BookingId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? RecipientUserId { get; set; }
    public short TypeId { get; set; }
    public short ChannelId { get; set; }
    public short StatusId { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public string? FailureReason { get; set; }
    public string? ProviderMessageId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Booking? Booking { get; set; }
    public Customer? Customer { get; set; }
    public User? RecipientUser { get; set; }
    public NotificationType Type { get; set; } = null!;
    public NotificationChannel Channel { get; set; } = null!;
    public NotificationStatus Status { get; set; } = null!;
}
