using BangaloreTaxi.Api.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BangaloreTaxi.Api.Hosting;

public sealed class PostgresReadyHealthCheck(IServiceScopeFactory scopes) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        await using var scope = scopes.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BangaloreTaxiDbContext>();
        try
        {
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("PostgreSQL is reachable.")
                : HealthCheckResult.Unhealthy("PostgreSQL is unreachable.");
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unreachable.");
        }
    }
}
