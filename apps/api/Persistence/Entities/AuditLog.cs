using System.Net;

namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class AuditLog
{
    public long Id { get; set; }
    public Guid? ActorUserId { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public IPAddress? IpAddress { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User? ActorUser { get; set; }
}
