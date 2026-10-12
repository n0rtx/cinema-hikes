using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

/// <summary>
/// Entity Framework Core implementation of <see cref="IMovieRepository"/> for catalog movies.
/// </summary>
public class MovieRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<Movie>(dbContext), IMovieRepository
{
    /// <summary>
    /// Loads a movie by ID including genres, video sources, links with translation studios, and user reviews.
    /// </summary>
    /// <param name="id">The unique identifier of the movie.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The movie with related data included, or <c>null</c> if not found.</returns>
    public async Task<Movie?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await DbContext.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.VideoSources)
            .Include(m => m.MovieLinks).ThenInclude(ml => ml.TranslationStudio)
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
}