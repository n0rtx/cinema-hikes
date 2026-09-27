using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces.Repositories;

public interface IGenreRepository : IRepository<Genre>
{
    Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}