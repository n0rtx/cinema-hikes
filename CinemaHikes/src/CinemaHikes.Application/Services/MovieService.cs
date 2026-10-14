using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.External;
using CinemaHikes.Domain.Specifications;

namespace CinemaHikes.Application.Services;

/// <summary>
/// Provides catalog query and search business logic for movies, with automatic caching of external metadata.
/// </summary>
public sealed class MovieService(
    IUnitOfWork uow,
    IMovieMetadataClient metadata) : IMovieService
{
    private IUnitOfWork Uow { get; } = uow;
    private IMovieMetadataClient Metadata { get; } = metadata;

    /// <summary>
    /// Retrieves a paginated list of movies based on genre, release year, and minimum rating.
    /// </summary>
    /// <param name="genreId">Optional identifier of the genre to filter movies by.</param>
    /// <param name="year">Optional release year to filter movies by.</param>
    /// <param name="minRating">Optional minimum rating threshold for filtering.</param>
    /// <param name="page">The 1-based page index.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of mapped <see cref="MovieListItemDto"/> matching the specification.</returns>
    public async Task<List<MovieListItemDto>> GetPagedAsync(
        int? genreId, int? year, double? minRating, int page, int pageSize, CancellationToken ct)
    {
        var spec = new MovieFilterSpecification(genreId, year, minRating, page, pageSize);
        var movies = await Uow.Movies.ListAsync(spec, ct);
        return movies.Select(MapListItem).ToList();
    }

    /// <summary>
    /// Searches for movies matching the query text locally, and falls back to external metadata search when no local matches exist.
    /// </summary>
    /// <param name="query">The text search query for movie title.</param>
    /// <param name="page">The 1-based page index.</param>
    /// <param name="pageSize">The number of results to fetch per page.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="MovieListItemDto"/> matching the query.</returns>
    public async Task<List<MovieListItemDto>> SearchAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        var local = await Uow.Movies.ListAsync(new MovieSearchSpecification(query, page, pageSize), ct);
        if (local.Count > 0)
            return local.Select(MapListItem).ToList();

        var external = await Metadata.SearchAsync(query, pageSize, ct);
        var saved = new List<Movie>();

        foreach (var item in external)
            saved.Add(await UpsertFromExternalAsync(item, ct));

        if (saved.Count > 0)
            await Uow.SaveChangesAsync(ct);

        return saved.Select(MapListItem).ToList();
    }

    /// <summary>
    /// Retrieves full movie details including genres, video sources, and reviews.
    /// </summary>
    /// <param name="id">The unique identifier of the movie.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="MovieDetailsDto"/> if found, or <c>null</c> if not found.</returns>
    public async Task<MovieDetailsDto?> GetDetailsAsync(int id, CancellationToken ct)
    {
        var movie = await Uow.Movies.GetWithDetailsAsync(id, ct);
        return movie is null ? null : MapDetails(movie);
    }

    private async Task<Movie> UpsertFromExternalAsync(ExternalMovieData data, CancellationToken ct)
    {
        var existing = (await Uow.Movies.ListAsync(new MovieSearchSpecification(data.RuTitle, 1, 5), ct))
            .FirstOrDefault(m => m.RuTitle == data.RuTitle && m.ReleaseYear == data.ReleaseYear);

        if (existing is not null)
            return existing;

        var movie = new Movie
        {
            RuTitle = data.RuTitle,
            UaTitle = data.UaTitle,
            RuInEngTitle = data.RuInEngTitle,
            Description = data.Description,
            Director = data.Director,
            ReleaseYear = data.ReleaseYear,
            PosterUrl = data.PosterUrl,
            KpRating = data.KpRating,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var genreName in data.GenreNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var genre = await Uow.Genres.GetByNameAsync(genreName, ct);
            if (genre is null)
            {
                genre = new Genre { Name = genreName };
                await Uow.Genres.AddAsync(genre, ct);
            }

            movie.MovieGenres.Add(new MovieGenre { Genre = genre, Movie = movie });
        }

        await Uow.Movies.AddAsync(movie, ct);
        return movie;
    }

    private static MovieListItemDto MapListItem(Movie m) => new(
        m.Id, m.RuTitle, m.UaTitle, m.RuInEngTitle, m.PosterUrl, m.ReleaseYear, m.KpRating,
        m.MovieGenres.Select(mg => new GenreDto(mg.Genre.Id, mg.Genre.Name)).ToList(),m.Director,m.Description);

    private static MovieDetailsDto MapDetails(Movie m) => new(
        m.Id, m.RuTitle, m.UaTitle, m.RuInEngTitle, m.Description, m.Director,
        m.ReleaseYear, m.PosterUrl, m.KpRating, m.CreatedAt,
        m.MovieGenres.Select(mg => new GenreDto(mg.Genre.Id, mg.Genre.Name)).ToList(),
        m.VideoSources.Select(vs => new VideoSourceDto(
            vs.Id, vs.MovieId, vs.ProviderName, vs.PageUrl, vs.Priority, vs.Status)).ToList(),
        m.MovieLinks.Select(ml => new MovieLinkDto(
            ml.Id, ml.MovieId,
            new TranslationStudioDto(ml.TranslationStudio.Id, ml.TranslationStudio.Name),
            ml.VideoQuality, ml.Url, ml.UpdatedAt)).ToList(),
        m.Reviews.Select(r => new ReviewDto(
            r.Id, r.MovieId, r.UserId, r.Text, r.Rating, r.Status, r.CreatedAt)).ToList());
}