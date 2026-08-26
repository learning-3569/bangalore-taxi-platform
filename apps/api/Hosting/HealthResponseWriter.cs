using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BangaloreTaxi.Api.Hosting;

public static class HealthResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task Write(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        var payload = new
        {
            status = report.Status.ToString().ToLowerInvariant(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString().ToLowerInvariant()
            })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    public static HealthCheckOptions LiveOptions() => new()
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = Write,
        AllowCachingResponses = false
    };

    public static HealthCheckOptions ReadyOptions() => new()
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = Write,
        AllowCachingResponses = false
    };
}
