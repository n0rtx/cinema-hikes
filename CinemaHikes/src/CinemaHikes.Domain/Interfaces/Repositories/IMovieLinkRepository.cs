using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for direct downloadable movie links and resolution caching.
/// </summary>
public interface IMovieLinkRepository : IRepository<MovieLink>
{
    /// <summary>
    /// Finds a cached direct video link by movie ID, video quality, and translation studio.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="quality">The requested video quality.</param>
    /// <param name="translationStudioId">The identifier of the voiceover/translation studio.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching <see cref="MovieLink"/> entity, or <c>null</c> if none matched.</returns>
    Task<MovieLink?> GetByMovieAndQualityAsync(
        int movieId, VideoQuality quality, int translationStudioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new movie link or updates an existing link matching the movie, quality, and studio composite key.
    /// </summary>
    /// <param name="movieLink">The movie link entity containing updated URL and timestamps.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpsertAsync(MovieLink movieLink, CancellationToken cancellationToken = default);
}