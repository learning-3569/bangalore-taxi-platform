namespace BangaloreTaxi.Api.Application;

public sealed class TooManyRequestsException : Exception
{
    public TooManyRequestsException(string message, int? retryAfterSeconds = null)
        : base(message)
    {
        RetryAfterSeconds = retryAfterSeconds is > 0 ? retryAfterSeconds : null;
    }

    public int? RetryAfterSeconds { get; }
}
