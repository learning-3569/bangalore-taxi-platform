namespace BangaloreTaxi.Api.Application;

public sealed class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message)
        : base(message)
    {
    }
}
