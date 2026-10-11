using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CinemaHikes.WebApi.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string FallbackJwtKey = "integration-test-jwt-key-32chars!!";

    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Key", FallbackJwtKey);
        Environment.SetEnvironmentVariable("Jwt__Issuer", "CinemaHikes");
        Environment.SetEnvironmentVariable("Jwt__Audience", "CinemaHikes");
        Environment.SetEnvironmentVariable("PoiskKino__ApiKey", "test-api-key");
        Environment.SetEnvironmentVariable("PoiskKino__BaseUrl", "https://api.poiskkino.dev/");
        Environment.SetEnvironmentVariable("ConnectionStrings__DatabaseConnection",
            "Host=localhost;Database=cinema_hikes_test;Username=test;Password=test");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            RemoveDbContextRegistrations(services);
            services.AddDbContext<CinemaHikesDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestsDb"));
        });
    }

    private static void RemoveDbContextRegistrations(IServiceCollection services)
    {
        services.RemoveAll(typeof(IDbContextOptionsConfiguration<CinemaHikesDbContext>));
        services.RemoveAll(typeof(DbContextOptions<CinemaHikesDbContext>));
        services.RemoveAll(typeof(DbContextOptions));
        services.RemoveAll(typeof(CinemaHikesDbContext));

        var leftovers = services
            .Where(d =>
                d.ServiceType == typeof(CinemaHikesDbContext) ||
                d.ServiceType == typeof(DbContextOptions<CinemaHikesDbContext>) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GenericTypeArguments.Any(t => t == typeof(CinemaHikesDbContext))))
            .ToList();

        foreach (var d in leftovers)
            services.Remove(d);
    }
}