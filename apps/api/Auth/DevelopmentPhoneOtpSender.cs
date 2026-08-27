using System.Collections.Concurrent;

namespace BangaloreTaxi.Api.Auth;

/// <summary>
/// In-memory OTP delivery for Development and Testing. Never register in Production.
/// </summary>
public sealed class DevelopmentPhoneOtpSender : IPhoneOtpSender
{
    private readonly ConcurrentDictionary<string, string> _latest = new();

    public Task SendAsync(string phoneE164, string otp, CancellationToken cancellationToken)
    {
        _latest[phoneE164] = otp;
        return Task.CompletedTask;
    }

    public bool TryPeek(string phoneE164, out string otp) => _latest.TryGetValue(phoneE164, out otp!);
}
