using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Users;

/// <summary>
/// Entity Framework Core generic repository implementation of <see cref="IUserMovieRelationRepository{T}"/>.
/// </summary>
/// <typeparam name="T">The entity type representing a user-movie relation.</typeparam>
public class UserMovieRelationRepository<T>(CinemaHikesDbContext dbContext)
    : IUserMovieRelationRepository<T> where T : class, IUserMovieRelation
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    /// <summary>
    /// Retrieves user movie relations including movie details and associated genre items.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of relations for the specified user.</returns>
    public async Task<List<T>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(e => e.Movie)
                .ThenInclude(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Finds a specific relation by user ID and movie ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The found relation entity, or <c>null</c> if not present.</returns>
    public async Task<T?> FindAsync(int userId, int movieId, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.UserId == userId && e.MovieId == movieId, cancellationToken);

    /// <summary>
    /// Asynchronously begins tracking a new relation in the Added state.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    /// <summary>
    /// Begins tracking the relation in the Deleted state.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public void Remove(T entity) => _dbSet.Remove(entity);
}