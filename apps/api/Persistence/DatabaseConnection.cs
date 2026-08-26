using Microsoft.EntityFrameworkCore;

namespace BangaloreTaxi.Api.Persistence;

public static class DatabaseConnection
{
    public const string DevelopmentFallback =
        "Host=127.0.0.1;Port=5432;Database=bangalore_taxi;Username=bangalore_taxi;Password=dev";

    public static void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options
            .UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history");
            })
            .UseSnakeCaseNamingConvention();
    }

    public static void Configure<TContext>(DbContextOptionsBuilder<TContext> options, string connectionString)
        where TContext : DbContext
    {
        Configure((DbContextOptionsBuilder)options, connectionString);
    }
}
