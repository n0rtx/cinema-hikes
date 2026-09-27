using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Dtos.Admin;

public sealed record AnalyticsSummaryDto(
    int TotalMovies,
    int TotalUsers,
    int TotalReviews,
    int TotalViews,
    IReadOnlyList<MovieListItemDto> TopMoviesByViews);