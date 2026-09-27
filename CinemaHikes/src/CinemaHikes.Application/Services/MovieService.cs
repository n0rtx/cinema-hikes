using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.External;
using CinemaHikes.Domain.Specifications;

namespace CinemaHikes.Application.Services;

public sealed class MovieService(
    IUnitOfWork uow,
    IMovieMetadataClient metadata) : IMovieService
{
    private IUnitOfWork Uow { get; } = uow;
    private IMovieMetadataClient Metadata { get; } = metadata;

    public async Task<List<MovieListItemDto>> GetPagedAsync(
        int? genreId, int? year, double? minRating, int page, int pageSize, CancellationToken ct)
    {
        var spec = new MovieFilterSpecification(genreId, year, minRating, page, pageSize);
        var movies = await Uow.Movies.ListAsync(spec, ct);
        return movies.Select(MapListItem).ToList();
    }

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
        m.MovieGenres.Select(mg => new GenreDto(mg.Genre.Id, mg.Genre.Name)).ToList());

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