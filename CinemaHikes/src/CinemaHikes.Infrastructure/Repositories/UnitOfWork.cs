using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;

namespace CinemaHikes.Infrastructure.Repositories;

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
    public IMovieRepository Movies { get; } = movies;

    public IGenreRepository Genres { get; } = genres;

    public IVideoSourceRepository VideoSources { get; } = videoSources;

    public IMovieLinkRepository MovieLinks { get; } = movieLinks;

    public ITranslationStudioRepository TranslationStudios { get; } = translationStudios;

    public IReviewRepository Reviews { get; } = reviews;

    public IParsingSourceRepository ParsingSources { get; } = parsingSources;

    public IUserMovieRelationRepository<FavoriteMovie> FavoriteMovies { get; } = favoriteMovies;

    public IUserMovieRelationRepository<ViewHistoryEntry> ViewHistory { get; } = viewHistory;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}