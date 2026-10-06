using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Bot;
using CinemaHikes.Domain.Interfaces.External;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Bot.Checkers;
using CinemaHikes.Infrastructure.Bot.Facades;
using CinemaHikes.Infrastructure.External.PoiskKino;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using CinemaHikes.Infrastructure.Repositories;
using CinemaHikes.Infrastructure.Repositories.Catalog;
using CinemaHikes.Infrastructure.Repositories.Parsing;
using CinemaHikes.Infrastructure.Repositories.Users;
using CinemaHikes.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
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

        services.AddDbContext<CinemaHikesDbContext>(options => options.UseNpgsql(connectionString));
        
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services
            .AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<CinemaHikesDbContext>();
        
        services.AddScoped<IMovieParserFacade, RezkaMovieParserFacade>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IVideoSourceRepository, VideoSourceRepository>();
        services.AddScoped<IMovieLinkRepository, MovieLinkRepository>();
        services.AddScoped<ITranslationStudioRepository, TranslationStudioRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IParsingSourceRepository, ParsingSourceRepository>();
        services.AddScoped(typeof(IUserMovieRelationRepository<>), typeof(UserMovieRelationRepository<>));
        
        services.AddScoped<IVideoSizeChecker, VideoSizeChecker>();
        services.AddHttpClient<IVideoSizeChecker, VideoSizeChecker>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<PoiskKinoOptions>(configuration.GetSection(PoiskKinoOptions.SectionName));
        services.AddHttpClient<IMovieMetadataClient, PoiskKinoClient>();

        return services;
    }
}