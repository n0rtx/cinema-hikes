using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaHikes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DatabaseConnection") ??
                                  throw new InvalidOperationException("No connection  string was found");

        services.AddDbContext<CinemaHikesDbContext>(options => options.UseNpgsql());

        return services;
    }
}