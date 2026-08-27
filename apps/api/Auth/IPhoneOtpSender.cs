namespace BangaloreTaxi.Api.Auth;

public interface IPhoneOtpSender
{
    Task SendAsync(string phoneE164, string otp, CancellationToken cancellationToken);
}
