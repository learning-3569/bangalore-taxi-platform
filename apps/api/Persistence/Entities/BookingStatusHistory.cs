namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class BookingStatusHistory
{
    public long Id { get; set; }
    public Guid BookingId { get; set; }
    public short? FromStatusId { get; set; }
    public short ToStatusId { get; set; }
    public Guid? ChangedByUserId { get; set; }
    public string? Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Booking Booking { get; set; } = null!;
    public BookingStatus? FromStatus { get; set; }
    public BookingStatus ToStatus { get; set; } = null!;
    public User? ChangedByUser { get; set; }
}
