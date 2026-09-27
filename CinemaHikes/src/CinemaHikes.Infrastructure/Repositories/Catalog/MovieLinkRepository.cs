using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

public class MovieLinkRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<MovieLink>(dbContext), IMovieLinkRepository
{
    public async Task<MovieLink?> GetByMovieAndQualityAsync(
        int movieId, VideoQuality quality, int translationStudioId, CancellationToken cancellationToken = default)
        => await DbContext.MovieLinks
            .FirstOrDefaultAsync(ml =>
                    ml.MovieId == movieId &&
                    ml.VideoQuality == quality &&
                    ml.TranslationStudioId == translationStudioId,
                cancellationToken);

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