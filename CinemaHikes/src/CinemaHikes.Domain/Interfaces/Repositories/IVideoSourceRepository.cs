using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for movie video source links and providers.
/// </summary>
public interface IVideoSourceRepository : IRepository<VideoSource>
{
    /// <summary>
    /// Retrieves all video source links configured for the given movie.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="VideoSource"/> entries for the movie.</returns>
    Task<List<VideoSource>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
}