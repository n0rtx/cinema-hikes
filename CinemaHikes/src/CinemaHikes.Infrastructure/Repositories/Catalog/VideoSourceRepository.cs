using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

/// <summary>
/// Entity Framework Core implementation of <see cref="IVideoSourceRepository"/> for video source links.
/// </summary>
public class VideoSourceRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<VideoSource>(dbContext), IVideoSourceRepository
{
    /// <summary>
    /// Retrieves all video sources for a movie sorted in ascending order of priority.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="VideoSource"/> records ordered by priority.</returns>
    public async Task<List<VideoSource>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        => await DbContext.VideoSources
            .Where(vs => vs.MovieId == movieId)
            .OrderBy(vs => vs.Priority)
            .ToListAsync(cancellationToken);
}