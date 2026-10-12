using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

/// <summary>
/// Entity Framework Core implementation of <see cref="IMovieLinkRepository"/> for direct movie stream links.
/// </summary>
public class MovieLinkRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<MovieLink>(dbContext), IMovieLinkRepository
{
    /// <summary>
    /// Finds a cached direct link for the given movie ID, resolution quality, and translation studio.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="quality">The video quality level.</param>
    /// <param name="translationStudioId">The identifier of the translation studio.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching <see cref="MovieLink"/> entity, or <c>null</c> if not cached.</returns>
    public async Task<MovieLink?> GetByMovieAndQualityAsync(
        int movieId, VideoQuality quality, int translationStudioId, CancellationToken cancellationToken = default)
        => await DbContext.MovieLinks
            .FirstOrDefaultAsync(ml =>
                    ml.MovieId == movieId &&
                    ml.VideoQuality == quality &&
                    ml.TranslationStudioId == translationStudioId,
                cancellationToken);

    /// <summary>
    /// Upserts a direct movie stream link, creating a new record or refreshing the URL and timestamp of an existing one.
    /// </summary>
    /// <param name="movieLink">The link entity to insert or update.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task UpsertAsync(MovieLink movieLink, CancellationToken cancellationToken = default)
    {
        var existing = await GetByMovieAndQualityAsync(
            movieLink.MovieId, movieLink.VideoQuality, movieLink.TranslationStudioId, cancellationToken);

        if (existing is null)
        {
            await DbSet.AddAsync(movieLink, cancellationToken);
        }
        else
        {
            existing.Url = movieLink.Url;
            existing.UpdatedAt = DateTime.UtcNow;
            DbSet.Update(existing);
        }
    }
}