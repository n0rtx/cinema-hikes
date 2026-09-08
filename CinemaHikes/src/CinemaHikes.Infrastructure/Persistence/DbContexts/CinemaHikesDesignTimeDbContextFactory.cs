using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CinemaHikes.Infrastructure.Persistence.DbContexts;

public class CinemaHikesDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CinemaHikesDbContext>
{
    public CinemaHikesDbContext CreateDbContext(string[] args)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<CinemaHikesDesignTimeDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configurationBuilder.GetConnectionString("DatabaseConnection")
                               ?? throw new InvalidOperationException("Connection string was not found.");

        var options = new DbContextOptionsBuilder<CinemaHikesDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CinemaHikesDbContext(options);
    }
}