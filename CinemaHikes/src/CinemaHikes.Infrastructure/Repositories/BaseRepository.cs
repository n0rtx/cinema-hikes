using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Domain.Specifications;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories;

/// <summary>
/// Generic base repository providing Entity Framework Core implementation of <see cref="IRepository{T}"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class BaseRepository<T>(CinemaHikesDbContext dbContext) : IRepository<T> where T : class
{
    /// <summary>
    /// The database context instance.
    /// </summary>
    protected readonly CinemaHikesDbContext DbContext = dbContext;

    /// <summary>
    /// The entity set corresponding to type <typeparamref name="T"/>.
    /// </summary>
    protected readonly DbSet<T> DbSet = dbContext.Set<T>();

    /// <summary>
    /// Finds an entity by primary key identifier using EF Core FindAsync.
    /// </summary>
    /// <param name="id">The primary key ID of the entity.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The found entity, or <c>null</c> if none matched.</returns>
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => await DbSet.FindAsync([id], cancellationToken);

    /// <summary>
    /// Evaluates the specification query with criteria, eager loading, ordering, and paging, returning results as a list.
    /// </summary>
    /// <param name="spec">The specification defining query requirements.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of entities matching the specification.</returns>
    public async Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        => await ApplySpecification(spec, true).ToListAsync(cancellationToken);

    /// <summary>
    /// Counts matching entities satisfying the specification criteria.
    /// </summary>
    /// <param name="spec">The specification containing filter expressions.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The total number of matching items in the database table.</returns>
    public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        => await ApplySpecification(spec, true).CountAsync(cancellationToken);

    /// <summary>
    /// Asynchronously begins tracking a new entity in the Added state.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddAsync(T entity, CancellationToken cancellationToken)
        => await DbSet.AddAsync(entity, cancellationToken);

    /// <summary>
    /// Begins tracking the entity in the Modified state.
    /// </summary>
    /// <param name="entity">The entity instance with updated fields.</param>
    public void Update(T entity) => DbSet.Update(entity);

    /// <summary>
    /// Begins tracking the entity in the Deleted state.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public void Remove(T entity) => DbSet.Remove(entity);

    private IQueryable<T> ApplySpecification(ISpecification<T> spec, bool applyPaging)
    {
        var query = DbSet.AsQueryable();

        if (spec.Criteria is not null) query = query.Where(spec.Criteria);

        foreach (var include in spec.Includes)
        {
            query = query.Include(include);
        }

        if (spec.OrderBy is not null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending is not null)
            query = query.OrderByDescending(spec.OrderByDescending);

        if (applyPaging && spec.IsPagingEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        return query;
    }
}