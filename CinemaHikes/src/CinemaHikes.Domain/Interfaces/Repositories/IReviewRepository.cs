using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for user movie reviews and moderation queries.
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Retrieves reviews for a movie, optionally filtered by moderation status (e.g., Pending, Approved, Rejected).
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="status">Optional moderation status filter.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="Review"/> entities for the movie.</returns>
    Task<List<Review>> GetByMovieIdAsync(
        int movieId, ReviewStatus? status = null, CancellationToken cancellationToken = default);
}