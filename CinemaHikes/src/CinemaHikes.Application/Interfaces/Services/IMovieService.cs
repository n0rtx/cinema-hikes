using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Interfaces.Services;


/// <summary>
/// Defines service operations for querying and searching movies in the catalog.
/// </summary>
public interface IMovieService
{
    /// <summary>
    /// Retrieves a paginated list of movies filtered by genre, release year, and minimum rating.
    /// </summary>
    /// <param name="genreId">Optional identifier of the genre to filter by.</param>
    /// <param name="year">Optional release year to filter by.</param>
    /// <param name="minRating">Optional minimum rating threshold.</param>
    /// <param name="page">The 1-based page index.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="MovieListItemDto"/> matching the filter criteria.</returns>
    Task<List<MovieListItemDto>> GetPagedAsync(
        int? genreId, int? year, double? minRating, int page, int pageSize, CancellationToken ct);

    /// <summary>
    /// Searches for movies by title query in the local repository and queries external metadata services if not found locally.
    /// </summary>
    /// <param name="query">The search term for the movie title.</param>
    /// <param name="page">The 1-based page index.</param>
    /// <param name="pageSize">The number of results per page.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="MovieListItemDto"/> matching the search query.</returns>
    Task<List<MovieListItemDto>> SearchAsync(string query, int page, int pageSize, CancellationToken ct);

    /// <summary>
    /// Retrieves comprehensive details for a specific movie including genres, video sources, and reviews.
    /// </summary>
    /// <param name="id">The unique identifier of the movie.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="MovieDetailsDto"/> if found, or <c>null</c> if no matching movie exists.</returns>
    Task<MovieDetailsDto?> GetDetailsAsync(int id, CancellationToken ct);
}