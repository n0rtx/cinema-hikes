using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

public class VideoSourceRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<VideoSource>(dbContext), IVideoSourceRepository
{
    public async Task<List<VideoSource>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        => await DbContext.VideoSources
            .Where(vs => vs.MovieId == movieId)
            .OrderBy(vs => vs.Priority)
            .ToListAsync(cancellationToken);
}