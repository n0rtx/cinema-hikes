using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Users;

public class UserMovieRelationRepository<T>(CinemaHikesDbContext dbContext)
    : IUserMovieRelationRepository<T> where T : class, IUserMovieRelation
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<List<T>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(e => e.UserId == userId).ToListAsync(cancellationToken);

    public async Task<T?> FindAsync(int userId, int movieId, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.UserId == userId && e.MovieId == movieId, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Remove(T entity) => _dbSet.Remove(entity);
}