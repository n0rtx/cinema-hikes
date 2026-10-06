using Scalar.AspNetCore;

namespace CinemaHikes.WebApi.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddControllers();

        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static WebApplication UseWebApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseCors("Frontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}