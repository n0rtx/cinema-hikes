using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    Task<Movie?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}