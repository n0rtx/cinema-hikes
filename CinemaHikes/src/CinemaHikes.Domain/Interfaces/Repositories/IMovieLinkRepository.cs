using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Interfaces.Repositories;

public interface IMovieLinkRepository : IRepository<MovieLink>
{
    Task<MovieLink?> GetByMovieAndQualityAsync(
        int movieId, VideoQuality quality, int translationStudioId, CancellationToken cancellationToken = default);

    Task UpsertAsync(MovieLink movieLink, CancellationToken cancellationToken = default);
}