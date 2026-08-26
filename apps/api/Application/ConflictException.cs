namespace BangaloreTaxi.Api.Application;

/// <summary>
/// Thrown when a write would violate a uniqueness or overlap rule.
/// Maps to HTTP 409. Future assignment services should throw this after catching
/// PostgreSQL exclusion/unique violations, or let <see cref="ExceptionHttpMapper"/> map them.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
