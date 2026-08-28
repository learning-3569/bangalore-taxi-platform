using System.Threading.RateLimiting;
using BangaloreTaxi.Api.Configuration;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Bookings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BangaloreTaxi.Api.Hosting;

public static class ServiceCollectionExtensions
{
    public const string CorsPolicyName = "FrontendApps";

    public static WebApplicationBuilder AddApiFoundation(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddHttpClient();

        builder.Services
            .AddOptions<OperationsOptions>()
            .Bind(builder.Configuration.GetSection(OperationsOptions.SectionName))
            .Validate(options => options.AssignmentBufferMinutes > 0, "Operations:AssignmentBufferMinutes must be positive.")
            .Validate(options => options.DefaultTripDurationMinutes > 0, "Operations:DefaultTripDurationMinutes must be positive.")
            .ValidateOnStart();

        builder.Services
            .AddOptions<CorsSettings>()
            .Bind(builder.Configuration.GetSection(CorsSettings.SectionName));

        builder.Services
            .AddOptions<RateLimitSettings>()
            .Bind(builder.Configuration.GetSection(RateLimitSettings.SectionName))
            .Validate(options => options.PermitLimit > 0, "RateLimiting:PermitLimit must be positive.")
            .ValidateOnStart();

        builder.AddPhoneOtpAuthentication();
        AddDatabase(builder);
        builder.Services.AddScoped<BookingService>();
        AddControllersAndProblemDetails(builder);
        AddCors(builder);
        AddRateLimiting(builder);
        AddHealthChecks(builder);
        AddSwagger(builder);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 1_048_576;
        });

        return builder;
    }

    private static void AddDatabase(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
            {
                connectionString = DatabaseConnection.DevelopmentFallback;
            }
            else
            {
                throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
            }
        }

        builder.Services.AddDbContext<BangaloreTaxiDbContext>(options =>
            DatabaseConnection.Configure(options, connectionString));
    }

    private static void AddControllersAndProblemDetails(WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            };
        });
        builder.Services.AddExceptionHandler<ApiExceptionHandler>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problem = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred.",
                    Type = "https://httpstatuses.io/400",
                    Instance = context.HttpContext.Request.Path.Value
                };
                problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                return new BadRequestObjectResult(problem)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });
    }

    private static void AddCors(WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration.GetSection($"{CorsSettings.SectionName}:AllowedOrigins").Get<string[]>()
            ?? [];

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                if (allowedOrigins.Length == 0)
                {
                    policy.SetIsOriginAllowed(_ => false);
                    return;
                }

                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private static void AddRateLimiting(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection(RateLimitSettings.SectionName).Get<RateLimitSettings>()
            ?? new RateLimitSettings();

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = settings.PermitLimit,
                    Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            options.AddPolicy("auth", httpContext =>
            {
                var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
            });

            options.AddPolicy("public-write", httpContext =>
            {
                var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
            });

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/problem+json";
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Too Many Requests",
                        Detail = "Too many verification requests. Please try again later.",
                        Type = "https://httpstatuses.io/429"
                    },
                    cancellationToken);
            };
        });
    }

    private static void AddHealthChecks(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"])
            .AddCheck<PostgresReadyHealthCheck>("postgres", tags: ["ready"]);
    }

    private static void AddSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Bangalore Taxi Platform API",
                Version = "v1",
                Description =
                    "Modular monolith API for the Bangalore Taxi Booking Platform. " +
                    "Phase 6 provides phone/OTP authentication and customer-owned booking requests. " +
                    "Online payment, pricing, assignment, and admin operations are excluded. " +
                    "Future resource routes use /api/v1/{resource}."
            });
        });
    }
}
