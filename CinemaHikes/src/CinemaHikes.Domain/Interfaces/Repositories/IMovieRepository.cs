using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for catalog movie queries and eager loading operations.
/// </summary>
public interface IMovieRepository : IRepository<Movie>
{
    /// <summary>
    /// Retrieves a movie by identifier including its navigation properties: genres, video sources, links, and reviews.
    /// </summary>
    /// <param name="id">The unique identifier of the movie.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The movie with details loaded, or <c>null</c> if not found.</returns>
    Task<Movie?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}