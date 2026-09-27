using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces.Repositories;

public interface IVideoSourceRepository : IRepository<VideoSource>
{
    Task<List<VideoSource>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
}