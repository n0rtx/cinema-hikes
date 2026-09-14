using CinemaHikes.Domain.Specifications;

namespace CinemaHikes.Domain.Interfaces.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    Task AddAsync(T entity, CancellationToken cancellationToken);

    void Update(T entity);

    void Remove(T entity);
}