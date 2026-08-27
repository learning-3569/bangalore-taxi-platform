namespace BangaloreTaxi.Api.Application;

public sealed class InvalidRequestException : Exception
{
    public InvalidRequestException(string message)
        : base(message)
    {
    }
}
