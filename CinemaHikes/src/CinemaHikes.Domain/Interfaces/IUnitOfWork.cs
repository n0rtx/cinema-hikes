using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces.Repositories;

namespace CinemaHikes.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface coordinating transactions and providing access to all repositories.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the movies repository.
    /// </summary>
    IMovieRepository Movies { get; }

    /// <summary>
    /// Gets the genres repository.
    /// </summary>
    IGenreRepository Genres { get; }

    /// <summary>
    /// Gets the video sources repository.
    /// </summary>
    IVideoSourceRepository VideoSources { get; }

    /// <summary>
    /// Gets the direct movie stream links repository.
    /// </summary>
    IMovieLinkRepository MovieLinks { get; }

    /// <summary>
    /// Gets the translation studios repository.
    /// </summary>
    ITranslationStudioRepository TranslationStudios { get; }

    /// <summary>
    /// Gets the movie reviews repository.
    /// </summary>
    IReviewRepository Reviews { get; }

    /// <summary>
    /// Gets the parsing sources configuration repository.
    /// </summary>
    IParsingSourceRepository ParsingSources { get; }

    /// <summary>
    /// Gets the favorite movies relation repository.
    /// </summary>
    IUserMovieRelationRepository<FavoriteMovie> FavoriteMovies { get; }

    /// <summary>
    /// Gets the viewing history relation repository.
    /// </summary>
    IUserMovieRelationRepository<ViewHistoryEntry> ViewHistory { get; }

    /// <summary>
    /// Commits all pending changes made in this unit of work to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}