using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Domain.Interfaces.Repositories;

public interface IUserMovieRelationRepository<T> where T : class, IUserMovieRelation
{
    Task<List<T>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<T?> FindAsync(int userId, int movieId, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Remove(T entity);
}