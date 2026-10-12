using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for movie genre queries.
/// </summary>
public interface IGenreRepository : IRepository<Genre>
{
    /// <summary>
    /// Finds a genre by its unique name string.
    /// </summary>
    /// <param name="name">The name of the genre.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching <see cref="Genre"/> entity, or <c>null</c> if not found.</returns>
    Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}