namespace BangaloreTaxi.Api.Persistence.Entities;

public sealed class User : IHasTimestamps
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneE164 { get; set; }
    public string? PasswordHash { get; set; }
    public short StatusId { get; set; }
    public DateTimeOffset? EmailConfirmedAt { get; set; }
    public DateTimeOffset? PhoneConfirmedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public UserStatus Status { get; set; } = null!;
    public Customer? Customer { get; set; }
    public Driver? Driver { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
