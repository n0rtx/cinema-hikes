using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CinemaHikesDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<CinemaHikesDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestsDb"));
        });
    }
}