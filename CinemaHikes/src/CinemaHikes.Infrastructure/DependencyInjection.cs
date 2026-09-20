using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using CinemaHikes.Infrastructure.Repositories;
using CinemaHikes.Infrastructure.Repositories.Catalog;
using CinemaHikes.Infrastructure.Repositories.Parsing;
using CinemaHikes.Infrastructure.Repositories.Users;
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
        
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IVideoSourceRepository, VideoSourceRepository>();
        services.AddScoped<IMovieLinkRepository, MovieLinkRepository>();
        services.AddScoped<ITranslationStudioRepository, TranslationStudioRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IParsingSourceRepository, ParsingSourceRepository>();
        services.AddScoped(typeof(IUserMovieRelationRepository<>), typeof(UserMovieRelationRepository<>));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}