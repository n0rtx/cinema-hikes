using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Generic repository interface for managing relations between users and movies (e.g., Favorites, View History).
/// </summary>
/// <typeparam name="T">The entity type implementing <see cref="IUserMovieRelation"/>.</typeparam>
public interface IUserMovieRelationRepository<T> where T : class, IUserMovieRelation
{
    /// <summary>
    /// Retrieves all relations for the specified user, including eager loaded movie details.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of relations of type <typeparamref name="T"/> for the user.</returns>
    Task<List<T>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a specific relation by user ID and movie ID composite key.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching relation entity, or <c>null</c> if none found.</returns>
    Task<T?> FindAsync(int userId, int movieId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new relation record to the persistence context.
    /// </summary>
    /// <param name="entity">The relation entity to add.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the relation record for deletion.
    /// </summary>
    /// <param name="entity">The relation entity to remove.</param>
    void Remove(T entity);
}