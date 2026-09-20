using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces.Repositories;

namespace CinemaHikes.Domain.Interfaces;

public interface IUnitOfWork
{
    IMovieRepository Movies { get; }

    IGenreRepository Genres { get; }

    IVideoSourceRepository VideoSources { get; }

    IMovieLinkRepository MovieLinks { get; }

    ITranslationStudioRepository TranslationStudios { get; }

    IReviewRepository Reviews { get; }

    IParsingSourceRepository ParsingSources { get; }

    IUserMovieRelationRepository<FavoriteMovie> FavoriteMovies { get; }

    IUserMovieRelationRepository<ViewHistoryEntry> ViewHistory { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}