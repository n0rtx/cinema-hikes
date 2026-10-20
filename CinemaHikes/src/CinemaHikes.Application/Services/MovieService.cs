using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Bot;
using CinemaHikes.Domain.Interfaces.External;
using CinemaHikes.Domain.Specifications;
using Microsoft.AspNetCore.Identity;

namespace CinemaHikes.Application.Services;

/// <summary>
/// Provides catalog query and search business logic for movies, with automatic caching of external metadata.
/// </summary>
public sealed class MovieService(
    IUnitOfWork uow,
    IMovieMetadataClient metadata,
    IOmdbClient omdb,
    UserManager<AppUser> userManager,
    IMovieParserFacade parser) : IMovieService
{
    private const string EmptyPlaceholder = "empty";
    private static readonly TimeSpan LinkTtl = TimeSpan.FromHours(24);

    private IUnitOfWork Uow { get; } = uow;
    private IMovieMetadataClient Metadata { get; } = metadata;
    private IOmdbClient Omdb { get; } = omdb;
    private UserManager<AppUser> UserManager { get; } = userManager;
    private IMovieParserFacade Parser { get; } = parser;

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
    /// <param name="userId">Optional identifier of the user who triggered the search; receives a hat when new movies are added from external metadata.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="MovieListItemDto"/> matching the query.</returns>
    public async Task<List<MovieListItemDto>> SearchAsync(
        string query, int page, int pageSize, int? userId, CancellationToken ct)
    {
        var local = await Uow.Movies.ListAsync(
            new MovieSearchSpecification(query, page, pageSize), ct);

        if (local.Count > 0)
        {
            var dirty = local.Where(NeedsEnrichment).ToList();
            foreach (var movie in dirty)
            {
                if (movie.CreatedAt == default)
                    movie.CreatedAt = DateTime.UtcNow;

                var candidates = await Metadata.SearchAsync(movie.RuTitle, 10, ct);
                var best = PickBestMatch(candidates, movie.RuTitle, movie.Director, movie.ReleaseYear);
                if (best is null)
                    continue;

                await ApplyMissingFieldsAsync(movie, best, ct);
            }

            if (dirty.Count > 0)
                await Uow.SaveChangesAsync(ct);

            return local.Select(MapListItem).ToList();
        }

        var external = await Metadata.SearchAsync(query, pageSize, ct);
        var saved = new List<Movie>();

        foreach (var item in external)
            saved.Add(await UpsertFromExternalAsync(item, ct));

        if (saved.Count > 0)
        {
            await Uow.SaveChangesAsync(ct);
            await AwardHatAsync(userId, ct);
        }

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
        if (movie is null)
            return null;

        // Считаем ДО обогащения: иначе после проставления CreatedAt ссылки в первый раз не получатся
        var needsLinks = NeedsLinkRefresh(movie);

        if (NeedsEnrichment(movie))
        {
            if (movie.CreatedAt == default)
                movie.CreatedAt = DateTime.UtcNow;

            var candidates = await Metadata.SearchAsync(movie.RuTitle, 10, ct);
            var best = PickBestMatch(candidates, movie.RuTitle, movie.Director, movie.ReleaseYear);
            if (best is not null)
                await ApplyMissingFieldsAsync(movie, best, ct);

            await Uow.SaveChangesAsync(ct);
        }

        if (needsLinks)
        {
            var sources = movie.VideoSources
                .Where(s => s.Status == SourceStatus.Active)
                .OrderBy(s => s.Priority)
                .ToList();

            if (sources.Count > 0)
            {
                var studio = await Uow.TranslationStudios.GetByNameAsync("Default", ct)
                             ?? new TranslationStudio { Name = "Default" };

                if (studio.Id == 0)
                    await Uow.TranslationStudios.AddAsync(studio, ct);

                foreach (var quality in new[]
                             { VideoQuality.FullHd, VideoQuality.High, VideoQuality.Medium, VideoQuality.Low })
                {
                    string? url = null;
                    foreach (var source in sources)
                    {
                        try
                        {
                            url = await Parser.GetMovieSrcAsync(source.PageUrl, quality);
                            if (!string.IsNullOrWhiteSpace(url))
                                break;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Не смог получить ссылку на фильм: {ex.Message}");
                            Console.ResetColor();
                        }
                    }

                    if (string.IsNullOrWhiteSpace(url))
                        continue;

                    await Uow.MovieLinks.UpsertAsync(new MovieLink
                    {
                        MovieId = movie.Id,
                        TranslationStudioId = studio.Id == 0 ? 1 : studio.Id,
                        VideoQuality = quality,
                        Url = url,
                        UpdatedAt = DateTime.UtcNow,
                        Movie = movie,
                        TranslationStudio = studio
                    }, ct);
                }

                await Uow.SaveChangesAsync(ct);
            }
        }

        return MapDetails(movie);
    }

    private static bool NeedsLinkRefresh(Movie movie) =>
        movie.CreatedAt == default
        || DateTime.UtcNow - movie.CreatedAt >= LinkTtl;

    private async Task AwardHatAsync(int? userId, CancellationToken ct)
    {
        if (userId is null)
            return;

        var user = await UserManager.FindByIdAsync(userId.Value.ToString());
        if (user is null)
            return;

        user.AmountOfHats += 1;
        await UserManager.UpdateAsync(user);
    }

    private async Task<Movie> UpsertFromExternalAsync(ExternalMovieData data, CancellationToken ct)
    {
        var existing = (await Uow.Movies.ListAsync(
                new MovieSearchSpecification(data.RuTitle, 1, 5), ct))
            .FirstOrDefault(m =>
                m.RuTitle == data.RuTitle && m.ReleaseYear == data.ReleaseYear);

        if (existing is not null)
        {
            if (NeedsEnrichment(existing))
                await ApplyMissingFieldsAsync(existing, data, ct);
            return existing;
        }

        var poster = await ResolvePosterFromPoiskDataAsync(data, ct);

        var movie = new Movie
        {
            RuTitle = CoalesceRequired(data.RuTitle),
            UaTitle = CoalesceRequired(data.UaTitle),
            RuInEngTitle = CoalesceRequired(data.RuInEngTitle),
            Description = CoalesceRequired(data.Description),
            Director = CoalesceRequired(data.Director),
            ReleaseYear = data.ReleaseYear,
            PosterUrl = CoalesceRequired(poster),
            KpRating = data.KpRating,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var genreName in data.GenreNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (IsMissing(genreName))
                continue;

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

    private async Task<string?> ResolvePosterFromPoiskDataAsync(ExternalMovieData data, CancellationToken ct)
    {
        if (data.ReleaseYear <= 0 || IsMissing(data.RuInEngTitle))
            return null;

        var poster = await Omdb.GetPosterByTitleAndYearAsync(
            data.RuInEngTitle.Trim(),
            data.ReleaseYear,
            ct);

        return IsMissing(poster) ? null : poster;
    }

    private static bool NeedsEnrichment(Movie m) =>
        IsMissing(m.UaTitle)
        || IsMissing(m.RuInEngTitle)
        || IsMissing(m.Description)
        || IsMissing(m.PosterUrl)
        || m.CreatedAt == default
        || m.KpRating <= 0.0
        || TitlesNeedFix(m);

    private static bool TitlesNeedFix(Movie m)
    {
        if (IsMissing(m.RuInEngTitle) || IsMissing(m.UaTitle))
            return true;

        var ru = m.RuTitle?.Trim() ?? string.Empty;
        var ua = m.UaTitle?.Trim() ?? string.Empty;
        var eng = m.RuInEngTitle?.Trim() ?? string.Empty;

        return string.Equals(ru, ua, StringComparison.OrdinalIgnoreCase)
               && string.Equals(ru, eng, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMissing(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        return string.Equals(value.Trim(), EmptyPlaceholder, StringComparison.OrdinalIgnoreCase);
    }

    private static string CoalesceRequired(string? value)
    {
        if (IsMissing(value))
            return EmptyPlaceholder;

        return value!.Trim();
    }

    private static ExternalMovieData? PickBestMatch(
        IReadOnlyList<ExternalMovieData> candidates,
        string dbTitle,
        string dbDirector,
        short dbYear)
    {
        if (candidates.Count == 0)
            return null;

        var title = Normalize(dbTitle);
        var director = Normalize(dbDirector);

        static bool Overlaps(string a, string b) =>
            a.Length > 0 && b.Length > 0
                         && (a.Contains(b, StringComparison.Ordinal) || b.Contains(a, StringComparison.Ordinal));

        ExternalMovieData? best = null;
        var bestScore = 0;

        foreach (var c in candidates)
        {
            var apiTitle = Normalize(c.RuTitle);
            var apiAlt = Normalize(c.RuInEngTitle);
            var apiDirector = Normalize(c.Director);

            var score = 0;

            if (title.Length > 0 && (apiTitle == title || apiAlt == title))
                score += 100;
            else if (Overlaps(title, apiTitle) || Overlaps(title, apiAlt))
                score += 50;

            if (director.Length > 0 && apiDirector.Length > 0
                                    && director != "unknown")
            {
                if (apiDirector == director)
                    score += 80;
                else if (Overlaps(director, apiDirector))
                    score += 40;
            }

            if (dbYear > 0 && c.ReleaseYear == dbYear)
                score += 30;

            if (score > bestScore)
            {
                bestScore = score;
                best = c;
            }
        }

        return bestScore >= 50 ? best : null;
    }

    private async Task ApplyMissingFieldsAsync(Movie movie, ExternalMovieData data, CancellationToken ct)
    {
        if (movie.CreatedAt == default)
            movie.CreatedAt = DateTime.UtcNow;

        if (IsMissing(movie.Description) && !IsMissing(data.Description))
            movie.Description = data.Description.Trim();

        if (movie.KpRating <= 0 && data.KpRating > 0)
            movie.KpRating = data.KpRating;

        if (IsMissing(movie.Director)
            || string.Equals(movie.Director, "Unknown", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsMissing(data.Director)
                && !string.Equals(data.Director, "Unknown", StringComparison.OrdinalIgnoreCase))
                movie.Director = data.Director.Trim();
        }

        if (IsMissing(movie.UaTitle) && !IsMissing(data.UaTitle))
            movie.UaTitle = data.UaTitle.Trim();

        var movieEng = movie.RuInEngTitle?.Trim();
        var movieRu = movie.RuTitle?.Trim();
        var dataEng = data.RuInEngTitle?.Trim();
        var dataRu = data.RuTitle?.Trim();

        var engNeedsFix = IsMissing(movieEng)
                          || string.Equals(movieEng, movieRu, StringComparison.OrdinalIgnoreCase);

        if (engNeedsFix
            && !IsMissing(dataEng)
            && !string.Equals(dataEng, dataRu, StringComparison.OrdinalIgnoreCase))
        {
            movie.RuInEngTitle = dataEng!;
        }

        if (IsMissing(movie.PosterUrl))
        {
            var lookup = data with
            {
                RuInEngTitle = IsMissing(movie.RuInEngTitle) ? data.RuInEngTitle : movie.RuInEngTitle,
                ReleaseYear = data.ReleaseYear > 0 ? data.ReleaseYear : movie.ReleaseYear
            };

            var poster = await ResolvePosterFromPoiskDataAsync(lookup, ct);
            if (!IsMissing(poster))
                movie.PosterUrl = poster!.Trim();
        }
    }

    private static string Normalize(string? value)
    {
        if (IsMissing(value))
            return string.Empty;

        return value!.Trim().ToLowerInvariant().Replace('ё', 'е');
    }

    private static MovieListItemDto MapListItem(Movie m) => new(
        m.Id, m.RuTitle, m.UaTitle, m.RuInEngTitle, m.PosterUrl, m.ReleaseYear, m.KpRating,
        m.MovieGenres.Select(mg => new GenreDto(mg.Genre.Id, mg.Genre.Name)).ToList(),
        m.Director, m.Description);

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