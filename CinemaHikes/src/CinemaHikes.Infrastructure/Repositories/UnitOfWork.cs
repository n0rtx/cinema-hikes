using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;

namespace CinemaHikes.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation aggregating all repository dependencies and coordinating database transactions via <see cref="CinemaHikesDbContext"/>.
/// </summary>
public sealed class UnitOfWork(
    CinemaHikesDbContext dbContext,
    IMovieRepository movies,
    IGenreRepository genres,
    IVideoSourceRepository videoSources,
    IMovieLinkRepository movieLinks,
    ITranslationStudioRepository translationStudios,
    IReviewRepository reviews,
    IParsingSourceRepository parsingSources,
    IUserMovieRelationRepository<FavoriteMovie> favoriteMovies,
    IUserMovieRelationRepository<ViewHistoryEntry> viewHistory) : IUnitOfWork
{
    /// <summary>
    /// Gets the movies repository instance.
    /// </summary>
    public IMovieRepository Movies { get; } = movies;

    /// <summary>
    /// Gets the genres repository instance.
    /// </summary>
    public IGenreRepository Genres { get; } = genres;

    /// <summary>
    /// Gets the video sources repository instance.
    /// </summary>
    public IVideoSourceRepository VideoSources { get; } = videoSources;

    /// <summary>
    /// Gets the direct movie stream links repository instance.
    /// </summary>
    public IMovieLinkRepository MovieLinks { get; } = movieLinks;

    /// <summary>
    /// Gets the translation studios repository instance.
    /// </summary>
    public ITranslationStudioRepository TranslationStudios { get; } = translationStudios;

    /// <summary>
    /// Gets the movie reviews repository instance.
    /// </summary>
    public IReviewRepository Reviews { get; } = reviews;

    /// <summary>
    /// Gets the parsing sources configuration repository instance.
    /// </summary>
    public IParsingSourceRepository ParsingSources { get; } = parsingSources;

    /// <summary>
    /// Gets the favorite movies relation repository instance.
    /// </summary>
    public IUserMovieRelationRepository<FavoriteMovie> FavoriteMovies { get; } = favoriteMovies;

    /// <summary>
    /// Gets the viewing history relation repository instance.
    /// </summary>
    public IUserMovieRelationRepository<ViewHistoryEntry> ViewHistory { get; } = viewHistory;

    /// <summary>
    /// Saves all changes tracked in the underlying database context asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}