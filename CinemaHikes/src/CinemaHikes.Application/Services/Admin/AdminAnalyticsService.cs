using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Specifications;

namespace CinemaHikes.Application.Services.Admin;

/// <summary>
/// Provides administrative reporting and analytics services.
/// </summary>
public sealed class AdminAnalyticsService(IUnitOfWork uow) : IAdminAnalyticsService
{
    private IUnitOfWork Uow { get; } = uow;

    /// <summary>
    /// Computes platform metrics including total movies, total reviews, and top 10 movies.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="AnalyticsSummaryDto"/> containing platform summary statistics.</returns>
    public async Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken ct)
    {
        var top = await Uow.Movies.ListAsync(
            new MovieFilterSpecification(
                genreIds: null, year: null, minRating: null, page: 1, pageSize: 10), ct);
        var movieCount = await Uow.Movies.CountAsync(new EmptySpecification<Movie>(), ct);
        var reviewCount = await Uow.Reviews.CountAsync(new EmptySpecification<Review>(), ct);

        return new AnalyticsSummaryDto(
            movieCount,
            0,
            reviewCount,
            0,
            top.Select(m => new MovieListItemDto(
                m.Id, m.RuTitle, m.UaTitle, m.RuInEngTitle, m.PosterUrl, m.ReleaseYear, m.KpRating,
                m.MovieGenres.Select(mg => new GenreDto(mg.Genre.Id, mg.Genre.Name)).ToList(), m.Director,
                m.Description)).ToList());
    }
}