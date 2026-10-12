using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

/// <summary>
/// Entity Framework Core implementation of <see cref="IReviewRepository"/> for user reviews.
/// </summary>
public class ReviewRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<Review>(dbContext), IReviewRepository
{
    /// <summary>
    /// Retrieves user reviews for a movie optionally filtered by status, ordered descending by creation date.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="status">Optional moderation status filter.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="Review"/> records.</returns>
    public async Task<List<Review>> GetByMovieIdAsync(
        int movieId, ReviewStatus? status = null, CancellationToken cancellationToken = default)
        => await DbContext.Reviews
            .Where(r => r.MovieId == movieId && (!status.HasValue || r.Status == status))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
}