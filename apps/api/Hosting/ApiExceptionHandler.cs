using BangaloreTaxi.Api.Application;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BangaloreTaxi.Api.Hosting;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger, IHostEnvironment environment)
    : IExceptionHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var mapped = ExceptionHttpMapper.Map(exception);
        var traceId = httpContext.TraceIdentifier;

        if (mapped.Status >= 500)
        {
            logger.LogError(exception, "Unhandled exception for {TraceId}", traceId);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception {Status} for {TraceId}", mapped.Status, traceId);
        }

        var detail = mapped.Detail;
        if (environment.IsDevelopment() && mapped.Status >= 500)
        {
            detail = exception.Message;
        }

        var problem = new ProblemDetails
        {
            Status = mapped.Status,
            Title = mapped.Title,
            Detail = detail,
            Type = $"https://httpstatuses.io/{mapped.Status}",
            Instance = httpContext.Request.Path.Value
        };
        problem.Extensions["traceId"] = traceId;
        if (mapped.RetryAfterSeconds is int retryAfter)
        {
            httpContext.Response.Headers.RetryAfter = retryAfter.ToString();
            problem.Extensions["retryAfterSeconds"] = retryAfter;
        }

        httpContext.Response.StatusCode = mapped.Status;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions), cancellationToken);
        return true;
    }
}
