using CinemaHikes.Application.Interfaces.Auth;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Application.Services;
using CinemaHikes.Application.Services.Admin;
using CinemaHikes.Application.Services.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaHikes.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IViewHistoryService, ViewHistoryService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IMovieDownloadService, MovieDownloadService>();

        services.AddScoped<IAdminMovieService, AdminMovieService>();
        services.AddScoped<IAdminSourceService, AdminSourceService>();
        services.AddScoped<IAdminAnalyticsService, AdminAnalyticsService>();
        services.AddScoped<IAdminUserService, AdminUserService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}