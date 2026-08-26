using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BangaloreTaxi.Api.Persistence;

public sealed class BangaloreTaxiDbContextFactory : IDesignTimeDbContextFactory<BangaloreTaxiDbContext>
{
    public BangaloreTaxiDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? DatabaseConnection.DevelopmentFallback;

        var options = new DbContextOptionsBuilder<BangaloreTaxiDbContext>();
        DatabaseConnection.Configure(options, connectionString);
        return new BangaloreTaxiDbContext(options.Options);
    }
}
