using System.Net;
using System.Text.Json;
using BangaloreTaxi.Api.Application;
using BangaloreTaxi.Api.Configuration;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BangaloreTaxi.Api.Auth;

public sealed class AuthService
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly BangaloreTaxiDbContext _db;
    private readonly IPhoneOtpSender _otpSender;
    private readonly AccessTokenIssuer _tokens;
    private readonly TimeProvider _clock;
    private readonly AuthOptions _options;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        BangaloreTaxiDbContext db,
        IPhoneOtpSender otpSender,
        AccessTokenIssuer tokens,
        TimeProvider clock,
        IOptions<AuthOptions> options,
        ILogger<AuthService> logger)
    {
        _db = db;
        _otpSender = otpSender;
        _tokens = tokens;
        _clock = clock;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<OtpRequestResponse> RequestOtpAsync(string phoneNumber, string? ip, CancellationToken cancellationToken)
    {
        if (!PhoneNormalizer.TryNormalize(phoneNumber, out var phone))
        {
            throw new InvalidRequestException("Enter a valid mobile number.");
        }

        var now = _clock.GetUtcNow();
        var cooldownFrom = now.AddSeconds(-_options.Otp.ResendCooldownSeconds);
        var latest = await _db.OtpChallenges
            .Where(c => c.PhoneE164 == phone)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is not null && latest.CreatedAt > cooldownFrom)
        {
            throw new TooManyRequestsException(
                "Please wait before requesting another code.",
                RetrySeconds(latest.CreatedAt.AddSeconds(_options.Otp.ResendCooldownSeconds), now));
        }

        var hourAgo = now.AddHours(-1);
        var hourlyWindow = await _db.OtpChallenges
            .Where(c => c.PhoneE164 == phone && c.CreatedAt > hourAgo)
            .OrderBy(c => c.CreatedAt)
            .Select(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
        if (hourlyWindow.Count >= _options.Otp.MaxRequestsPerHour)
        {
            throw new TooManyRequestsException(
                "Please wait before requesting another code.",
                RetrySeconds(hourlyWindow[0].AddHours(1), now));
        }

        var active = await _db.OtpChallenges
            .Where(c => c.PhoneE164 == phone && c.ConsumedAt == null)
            .ToListAsync(cancellationToken);
        foreach (var row in active)
        {
            row.ConsumedAt = now;
        }

        var otp = OtpDigest.NewOtp(_options.Otp.Length);
        var salt = OtpDigest.NewSalt();
        var challenge = new OtpChallenge
        {
            Id = Guid.NewGuid(),
            PhoneE164 = phone,
            Salt = salt,
            CodeHash = OtpDigest.Hash(otp, salt, _options.Otp.Pepper),
            ExpiresAt = now.AddSeconds(_options.Otp.ExpirySeconds),
            CreatedAt = now,
            RequestIp = TrimIp(ip)
        };
        _db.OtpChallenges.Add(challenge);

        try
        {
            await _otpSender.SendAsync(phone, otp, cancellationToken);
        }
        catch
        {
            challenge.ConsumedAt = now;
            await _db.SaveChangesAsync(cancellationToken);
            throw;
        }

        await AuditAsync(null, "otp_requested", "otp_challenge", challenge.Id, ip, new { phoneLast4 = PhoneNormalizer.LastFour(phone) }, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("OTP requested for phone ending {Last4} {Trace}", PhoneNormalizer.LastFour(phone), challenge.Id);
        return new OtpRequestResponse { Ok = true, ResendAvailableInSeconds = _options.Otp.ResendCooldownSeconds };
    }

    public async Task<AuthSessionResult> VerifyOtpAsync(
        string phoneNumber,
        string otp,
        string? ip,
        string? userAgent,
        bool includeRefreshToken,
        CancellationToken cancellationToken)
    {
        if (!PhoneNormalizer.TryNormalize(phoneNumber, out var phone))
        {
            throw new InvalidRequestException("Enter a valid mobile number.");
        }

        var now = _clock.GetUtcNow();
        var challenge = await _db.OtpChallenges
            .Where(c => c.PhoneE164 == phone && c.ConsumedAt == null)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (challenge is null || challenge.ExpiresAt <= now || challenge.AttemptCount >= _options.Otp.MaxAttempts)
        {
            throw new UnauthorizedException("Unable to verify the code.");
        }

        if (!OtpDigest.Equals(otp.Trim(), challenge.Salt, _options.Otp.Pepper, challenge.CodeHash))
        {
            challenge.AttemptCount++;
            if (challenge.AttemptCount >= _options.Otp.MaxAttempts)
            {
                challenge.ConsumedAt = now;
                await AuditAsync(null, "otp_verify_locked", "otp_challenge", challenge.Id, ip, new { phoneLast4 = PhoneNormalizer.LastFour(phone) }, cancellationToken);
            }
            else
            {
                await AuditAsync(null, "otp_verify_failed", "otp_challenge", challenge.Id, ip, new { attempts = challenge.AttemptCount }, cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Unable to verify the code.");
        }

        challenge.ConsumedAt = now;

        var user = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Customer)
            .SingleOrDefaultAsync(u => u.PhoneE164 == phone, cancellationToken);

        var created = false;
        if (user is null)
        {
            user = await CreateCustomerUserAsync(phone, now, cancellationToken);
            created = true;
        }
        else
        {
            await EnsureCustomerRoleAsync(user, now, cancellationToken);
            user.PhoneConfirmedAt ??= now;
        }

        var roles = user.UserRoles.Select(r => r.Role.Code).Distinct().ToList();
        var (access, expires) = _tokens.Issue(user, roles, user.Customer?.Id);
        var refresh = OtpDigest.NewRefreshToken();
        var session = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = OtpDigest.HashRefreshToken(refresh),
            ExpiresAt = now.AddDays(_options.RefreshTokenDays),
            CreatedAt = now,
            RequestIp = TrimIp(ip),
            UserAgent = TrimUa(userAgent)
        };
        _db.RefreshSessions.Add(session);

        await AuditAsync(user.Id, "otp_verified", "users", user.Id, ip, new { created }, cancellationToken);
        await AuditAsync(user.Id, "session_created", "refresh_session", session.Id, ip, null, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new AuthSessionResult(
            ToUserResponse(user, roles),
            access,
            expires,
            includeRefreshToken ? refresh : null,
            refresh,
            session.Id);
    }

    public async Task<AuthSessionResult> RefreshAsync(
        string refreshToken,
        string? ip,
        string? userAgent,
        bool includeRefreshToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedException("Session is no longer valid.");
        }

        var hash = OtpDigest.HashRefreshToken(refreshToken);
        var now = _clock.GetUtcNow();
        var session = await _db.RefreshSessions
            .Include(s => s.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(s => s.User).ThenInclude(u => u.Customer)
            .SingleOrDefaultAsync(s => s.TokenHash == hash, cancellationToken);

        if (session is null)
        {
            throw new UnauthorizedException("Session is no longer valid.");
        }

        if (session.RevokedAt is not null)
        {
            if (session.ReplacedById is not null)
            {
                var others = await _db.RefreshSessions
                    .Where(s => s.UserId == session.UserId && s.RevokedAt == null)
                    .ToListAsync(cancellationToken);
                foreach (var row in others)
                {
                    row.RevokedAt = now;
                }

                await AuditAsync(session.UserId, "refresh_replay", "refresh_session", session.Id, ip, null, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }

            throw new UnauthorizedException("Session is no longer valid.");
        }

        if (session.ExpiresAt <= now)
        {
            session.RevokedAt = now;
            await _db.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Session is no longer valid.");
        }

        var rotated = OtpDigest.NewRefreshToken();
        var replacement = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = session.UserId,
            TokenHash = OtpDigest.HashRefreshToken(rotated),
            ExpiresAt = now.AddDays(_options.RefreshTokenDays),
            CreatedAt = now,
            RequestIp = TrimIp(ip),
            UserAgent = TrimUa(userAgent)
        };
        session.RevokedAt = now;
        session.ReplacedById = replacement.Id;
        _db.RefreshSessions.Add(replacement);

        var roles = session.User.UserRoles.Select(r => r.Role.Code).Distinct().ToList();
        var (access, expires) = _tokens.Issue(session.User, roles, session.User.Customer?.Id);
        await _db.SaveChangesAsync(cancellationToken);

        return new AuthSessionResult(
            ToUserResponse(session.User, roles),
            access,
            expires,
            includeRefreshToken ? rotated : null,
            rotated,
            replacement.Id);
    }

    public async Task LogoutAsync(string? refreshToken, Guid? userId, string? ip, CancellationToken cancellationToken)
    {
        var now = _clock.GetUtcNow();
        RefreshSession? session = null;
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var hash = OtpDigest.HashRefreshToken(refreshToken);
            session = await _db.RefreshSessions.SingleOrDefaultAsync(s => s.TokenHash == hash, cancellationToken);
        }

        if (session is not null && session.RevokedAt is null)
        {
            session.RevokedAt = now;
            await AuditAsync(session.UserId, "logout", "refresh_session", session.Id, ip, null, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return;
        }

        if (userId is Guid id)
        {
            await AuditAsync(id, "logout", "users", id, ip, null, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<AuthUserResponse> GetMeAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Customer)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new UnauthorizedException("Session is no longer valid.");

        var roles = user.UserRoles.Select(r => r.Role.Code).Distinct().ToList();
        return ToUserResponse(user, roles);
    }

    public string? PeekDevelopmentOtp(string phoneNumber)
    {
        if (!PhoneNormalizer.TryNormalize(phoneNumber, out var phone))
        {
            return null;
        }

        return _otpSender is DevelopmentPhoneOtpSender dev && dev.TryPeek(phone, out var otp) ? otp : null;
    }

    private async Task<User> CreateCustomerUserAsync(string phone, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            PhoneE164 = phone,
            StatusId = ReferenceData.UserStatusActive,
            PhoneConfirmedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DisplayName = "Customer " + PhoneNormalizer.LastFour(phone),
            StatusId = ReferenceData.CustomerStatusActive,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Users.Add(user);
        _db.Customers.Add(customer);
        _db.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = ReferenceData.RoleIds.Customer,
            AssignedAt = now
        });
        await _db.SaveChangesAsync(cancellationToken);
        await _db.Entry(user).Collection(u => u.UserRoles).Query().Include(ur => ur.Role).LoadAsync(cancellationToken);
        await _db.Entry(user).Reference(u => u.Customer).LoadAsync(cancellationToken);
        return user;
    }

    private async Task EnsureCustomerRoleAsync(User user, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (user.UserRoles.All(r => r.RoleId != ReferenceData.RoleIds.Customer))
        {
            _db.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = ReferenceData.RoleIds.Customer,
                AssignedAt = now
            });
        }

        if (user.Customer is null)
        {
            user.Customer = new Customer
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DisplayName = "Customer " + PhoneNormalizer.LastFour(user.PhoneE164 ?? ""),
                StatusId = ReferenceData.CustomerStatusActive,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Customers.Add(user.Customer);
        }

        await Task.CompletedTask;
    }

    private async Task AuditAsync(
        Guid? actorId,
        string action,
        string entityType,
        Guid entityId,
        string? ip,
        object? details,
        CancellationToken cancellationToken)
    {
        IPAddress? parsed = null;
        if (!string.IsNullOrWhiteSpace(ip) && IPAddress.TryParse(ip.Split(',')[0].Trim(), out var addr))
        {
            parsed = addr;
        }

        _db.AuditLogs.Add(new AuditLog
        {
            ActorUserId = actorId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            NewValue = details is null ? null : JsonSerializer.Serialize(details, Json),
            IpAddress = parsed,
            CreatedAt = _clock.GetUtcNow()
        });
        await Task.CompletedTask;
    }

    private static AuthUserResponse ToUserResponse(User user, IReadOnlyList<string> roles)
    {
        var phone = user.PhoneE164 ?? "";
        return new AuthUserResponse
        {
            UserId = user.Id,
            CustomerId = user.Customer?.Id,
            PhoneNumber = phone,
            MaskedPhone = phone.Length >= 4 ? "******" + PhoneNormalizer.LastFour(phone) : phone,
            Roles = roles
        };
    }

    private static int RetrySeconds(DateTimeOffset until, DateTimeOffset now)
    {
        return Math.Max(1, (int)Math.Ceiling((until - now).TotalSeconds));
    }

    private static string? TrimIp(string? ip) => string.IsNullOrWhiteSpace(ip) ? null : ip.Split(',')[0].Trim()[..Math.Min(64, ip.Split(',')[0].Trim().Length)];

    private static string? TrimUa(string? ua) => string.IsNullOrWhiteSpace(ua) ? null : ua[..Math.Min(256, ua.Length)];
}

public sealed record AuthSessionResult(
    AuthUserResponse User,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string? RefreshTokenForBody,
    string RefreshTokenForCookie,
    Guid SessionId);
