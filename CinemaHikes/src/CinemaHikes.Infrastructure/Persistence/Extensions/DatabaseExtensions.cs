using CinemaHikes.Infrastructure.Persistence.DbContexts;
using CinemaHikes.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CinemaHikes.Infrastructure.Persistence.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider rootServices, CancellationToken cancellationToken)
    {
        using var scope = rootServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseInitializer");

        try
        {
            var db = services.GetRequiredService<CinemaHikesDbContext>();
            await db.Database.MigrateAsync(cancellationToken);
            await CatalogSeeder.SeedAsync(services, cancellationToken);
            logger.LogInformation("Database migrated and seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}