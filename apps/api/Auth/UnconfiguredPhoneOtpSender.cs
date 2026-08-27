using BangaloreTaxi.Api.Application;

namespace BangaloreTaxi.Api.Auth;

public sealed class UnconfiguredPhoneOtpSender : IPhoneOtpSender
{
    public Task SendAsync(string phoneE164, string otp, CancellationToken cancellationToken)
    {
        throw new ServiceUnavailableException("SMS delivery is not configured.");
    }
}
