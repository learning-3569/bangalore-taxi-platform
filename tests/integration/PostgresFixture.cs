using BangaloreTaxi.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace BangaloreTaxi.IntegrationTests;

public sealed class PostgresFixture : IAsyncLifetime
{
    public const string LocalDevelopmentConnection =
        "Host=127.0.0.1;Port=5432;Database=bangalore_taxi;Username=bangalore_taxi;Password=dev";

    public const string LocalTestDatabase = "bangalore_taxi_test";

    private PostgreSqlContainer? _container;

    public bool IsAvailable { get; private set; }

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("SCHEMA_TEST_CONNECTION");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            ConnectionString = fromEnvironment;
            IsAvailable = true;
            await MigrateAsync();
            return;
        }

        if (await TryUseLocalComposeDatabaseAsync())
        {
            return;
        }

        try
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .WithUsername("bangalore_taxi")
                .WithPassword("dev")
                .WithDatabase("bangalore_taxi_test")
                .Build();
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();
            IsAvailable = true;
            await MigrateAsync();
        }
        catch (Exception)
        {
            IsAvailable = false;
        }
    }

    public BangaloreTaxiDbContext CreateContext()
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("PostgreSQL is not available for schema tests.");
        }

        var options = new DbContextOptionsBuilder<BangaloreTaxiDbContext>();
        DatabaseConnection.Configure(options, ConnectionString);
        return new BangaloreTaxiDbContext(options.Options);
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }

    private async Task<bool> TryUseLocalComposeDatabaseAsync()
    {
        try
        {
            await using var admin = new NpgsqlConnection(LocalDevelopmentConnection);
            await admin.OpenAsync();
            await using (var create = new NpgsqlCommand(
                             $"SELECT 1 FROM pg_database WHERE datname = '{LocalTestDatabase}'",
                             admin))
            {
                var exists = await create.ExecuteScalarAsync();
                if (exists is null)
                {
                    await using var createDb = new NpgsqlCommand(
                        $"CREATE DATABASE {LocalTestDatabase} OWNER bangalore_taxi",
                        admin);
                    await createDb.ExecuteNonQueryAsync();
                }
            }

            ConnectionString =
                "Host=127.0.0.1;Port=5432;Database=bangalore_taxi_test;Username=bangalore_taxi;Password=dev";
            IsAvailable = true;
            await using var db = CreateContext();
            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
            return true;
        }
        catch (Exception)
        {
            IsAvailable = false;
            ConnectionString = string.Empty;
            return false;
        }
    }

    private async Task MigrateAsync()
    {
        await using var db = CreateContext();
        await db.Database.MigrateAsync();
    }
}

[CollectionDefinition("postgres")]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>;
