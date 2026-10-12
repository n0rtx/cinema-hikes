using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Interfaces.Services;

/// <summary>
/// Defines service operations for reading and publishing movie user reviews.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Retrieves all approved reviews posted for the specified movie.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="ReviewDto"/> containing review texts, ratings, authors, and timestamps.</returns>
    Task<List<ReviewDto>> GetByMovieIdAsync(int movieId, CancellationToken ct);

    /// <summary>
    /// Creates a new review and rating for a movie.
    /// </summary>
    /// <param name="userId">The unique identifier of the reviewing user.</param>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="text">The content text of the review.</param>
    /// <param name="rating">The numeric rating given by the user (typically between 1 and 10).</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task CreateAsync(int userId, int movieId, string text, double rating, CancellationToken ct);
}