using System.Net;
using System.Text;
using BangaloreTaxi.Api.Auth;
using BangaloreTaxi.Api.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;

namespace BangaloreTaxi.Api.Hosting;

public static class AuthServiceCollectionExtensions
{
    public static WebApplicationBuilder AddPhoneOtpAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<AuthOptions>()
            .Bind(builder.Configuration.GetSection(AuthOptions.SectionName))
            .Validate(options => options.Otp.Length is >= 4 and <= 8, "Auth:Otp:Length must be 4-8.")
            .Validate(options => options.Otp.ExpirySeconds > 0, "Auth:Otp:ExpirySeconds must be positive.")
            .Validate(options => options.Otp.MaxAttempts > 0, "Auth:Otp:MaxAttempts must be positive.")
            .Validate(options => options.Otp.Pepper.Length >= 32, "Auth:Otp:Pepper must be at least 32 characters.")
            .Validate(options => options.Jwt.SigningKey.Length >= 32, "Auth:Jwt:SigningKey must be at least 32 characters.")
            .Validate(options =>
                    !builder.Environment.IsProduction()
                    || !string.Equals(options.Otp.Provider, "Development", StringComparison.OrdinalIgnoreCase),
                "Auth:Otp:Provider cannot be Development in Production.")
            .ValidateOnStart();

        builder.Services.AddSingleton<DevelopmentPhoneOtpSender>();
        builder.Services.AddSingleton<UnconfiguredPhoneOtpSender>();
        builder.Services.AddSingleton<IPhoneOtpSender>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthOptions>>().Value;
            var env = sp.GetRequiredService<IHostEnvironment>();
            var development = string.Equals(options.Otp.Provider, "Development", StringComparison.OrdinalIgnoreCase);
            if (env.IsProduction() && development)
            {
                throw new InvalidOperationException("Development OTP sender cannot be used in Production.");
            }

            if (development && (env.IsDevelopment() || env.IsEnvironment("Testing")))
            {
                return sp.GetRequiredService<DevelopmentPhoneOtpSender>();
            }

            return sp.GetRequiredService<UnconfiguredPhoneOtpSender>();
        });

        builder.Services.AddScoped<AccessTokenIssuer>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<AuthCookieService>();

        var jwt = builder.Configuration.GetSection($"{AuthOptions.SectionName}:Jwt").Get<JwtOptions>() ?? new JwtOptions();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        string.IsNullOrEmpty(jwt.SigningKey)
                            ? "development-only-placeholder-key-32ch"
                            : jwt.SigningKey)),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
                };
            });
        builder.Services.AddAuthorization();

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = 1;
            var settings = builder.Configuration.GetSection(ForwardedHeadersSettings.SectionName)
                .Get<ForwardedHeadersSettings>() ?? new ForwardedHeadersSettings();
            foreach (var proxy in settings.KnownProxies)
            {
                if (IPAddress.TryParse(proxy, out var address))
                {
                    options.KnownProxies.Add(address);
                }
            }

            foreach (var network in settings.KnownNetworks)
            {
                var parts = network.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2
                    && IPAddress.TryParse(parts[0], out var prefix)
                    && int.TryParse(parts[1], out var prefixLength))
                {
                    options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(prefix, prefixLength));
                }
            }
        });

        return builder;
    }
}
