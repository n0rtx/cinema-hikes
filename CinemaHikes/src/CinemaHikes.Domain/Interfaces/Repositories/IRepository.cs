using CinemaHikes.Domain.Specifications;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Generic repository interface defining standard CRUD and specification-based querying abstractions.
/// </summary>
/// <typeparam name="T">The entity class managed by the repository.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Finds and retrieves an entity by its integer primary key identifier.
    /// </summary>
    /// <param name="id">The primary key ID of the entity.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching entity instance, or <c>null</c> if no entity matches the identifier.</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Queries a list of entities satisfying the criteria, includes, sorting, and pagination of the specified specification.
    /// </summary>
    /// <param name="spec">The specification defining query criteria, expressions, and includes.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of matching entity instances.</returns>
    Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of entities that satisfy the given query specification.
    /// </summary>
    /// <param name="spec">The specification defining filter criteria.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The total number of matching entities.</returns>
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new entity to the underlying persistence context.
    /// </summary>
    /// <param name="entity">The entity instance to add.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Marks an entity as modified in the change tracker.
    /// </summary>
    /// <param name="entity">The entity instance to update.</param>
    void Update(T entity);

    /// <summary>
    /// Marks an entity for deletion from the persistence context.
    /// </summary>
    /// <param name="entity">The entity instance to remove.</param>
    void Remove(T entity);
}