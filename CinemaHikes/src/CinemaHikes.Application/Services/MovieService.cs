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
    IMovieParserFacade parserFacade) : IMovieService
{
    private IUnitOfWork Uow { get; } = uow;
    private IMovieMetadataClient Metadata { get; } = metadata;
    private IOmdbClient Omdb { get; } = omdb;
    private UserManager<AppUser> UserManager { get; } = userManager;
    private IMovieParserFacade ParserFacade { get; } = parserFacade;

    private static readonly TimeSpan LinkTtl = TimeSpan.FromHours(24);

    /// <summary>
    /// Retrieves a paginated list of movies based on genre, release year, and minimum rating.
    /// </summary>
    /// <param name="genreId">Optional identifier of the genre to filter movies by.</param>
    /// <param name="year">Optional release year to filter movies by.</param>
    /// <param name="minRating">Optional minimum rating threshold for filtering.</param>
    /// <param name="page">The 1-based page index.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="onlyWithPoster">Display only movies with the poster</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A paginated result of mapped <see cref="MovieListItemDto"/> matching the specification.</returns>
    public async Task<PagedResultDto<MovieListItemDto>> GetPagedAsync(
        IReadOnlyCollection<int>? genreIds, int? year, double? minRating,
        string? sort, bool matchAllGenres,
        int page, int pageSize, bool onlyWithPoster, CancellationToken ct)
    {
        var pagedSpec = new MovieFilterSpecification(
            genreIds, year, minRating, sort, matchAllGenres, page, pageSize, onlyWithPoster);
        var countSpec = new MovieFilterSpecification(
            genreIds, year, minRating, sort, matchAllGenres, onlyWithPoster: onlyWithPoster);

        var total = await Uow.Movies.CountAsync(countSpec, ct);
        var movies = await Uow.Movies.ListAsync(pagedSpec, ct);

        return new PagedResultDto<MovieListItemDto>(
            movies.Select(MapListItem).ToList(), total);
    }

    /// <summary>
    /// Searches for movies matching the query text locally, and falls back to external metadata search when no local matches exist.
    /// </summary>
    /// <param name="query">The text search query for movie title.</param>
    /// <param name="page">The 1-based page index.</param>
    /// <param name="pageSize">The number of results to fetch per page.</param>
    /// <param name="userId">Optional identifier of the user who triggered the search; receives a hat when new movies are added from external metadata.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A paginated result of <see cref="MovieListItemDto"/> matching the query.</returns>
    public async Task<PagedResultDto<MovieListItemDto>> SearchAsync(
        string query, int page, int pageSize, int? userId, CancellationToken ct)
    {
        var local = await Uow.Movies.ListAsync(
            new MovieSearchSpecification(query, page, pageSize), ct);

        if (local.Count > 0)
        {
            var total = await Uow.Movies.CountAsync(new MovieSearchSpecification(query), ct);

            return new PagedResultDto<MovieListItemDto>(
                local.Select(MapListItem).ToList(),
                total);
        }

        var external = await Metadata.SearchAsync(query, pageSize, ct);
        var genreMap = await LoadGenreMapAsync(ct);
        var saved = new List<Movie>();
        
        Console.WriteLine($"genreMap: {genreMap.Count} [{string.Join(", ", genreMap.Keys)}]");
        foreach (var item in external)
            Console.WriteLine($"{item.RuTitle}: genres = [{string.Join(", ", item.GenreNames)}]");
        
        foreach (var item in external)
            saved.Add(await UpsertFromExternalAsync(item, genreMap, ct));

        if (saved.Count > 0)
        {
            await Uow.SaveChangesAsync(ct);
            await AwardHatAsync(userId, ct);
        }

        return new PagedResultDto<MovieListItemDto>(
            saved.Select(MapListItem).ToList(),
            saved.Count);
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

        var needsLinks = NeedsLinkRefresh(movie);
        var needsGenres = needsLinks && movie.MovieGenres.Count == 0;
        var needsEnrichment = NeedsEnrichment(movie);

        if (needsEnrichment || needsGenres)
        {
            try
            {
                var candidates = await Metadata.SearchAsync(movie.RuTitle, 10, ct);
                var best = PickBestMatch(candidates, movie.RuTitle, movie.Director, movie.ReleaseYear);
                if (best is not null)
                {
                    if (needsEnrichment)
                        await ApplyMissingFieldsAsync(movie, best, ct);

                    if (needsGenres)
                        await ApplyGenresAsync(movie, best, ct);

                    await Uow.SaveChangesAsync(ct);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Metadata enrichment failed for movie {movie.Id}: {ex.Message}");
                Console.ResetColor();
            }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Metadata enrichment timed out for movie {movie.Id}: {ex.Message}");
                Console.ResetColor();
            }
        }

        if (needsLinks)
            await RefreshLinksFromRezkaAsync(movie, ct);

        return MapDetails(movie);
    }

    private async Task RefreshLinksFromRezkaAsync(Movie movie, CancellationToken ct)
    {
        var sources = movie.VideoSources
            .Where(s => s.Status == SourceStatus.Active)
            .OrderBy(s => s.Priority)
            .ToList();

        if (sources.Count == 0)
            return;

        var studio = await Uow.TranslationStudios.GetByNameAsync("Default", ct)
                     ?? new TranslationStudio { Name = "Default" };

        if (studio.Id == 0)
            await Uow.TranslationStudios.AddAsync(studio, ct);

        var qualities = new[]
        {
            VideoQuality.FullHd,
            VideoQuality.High,
            VideoQuality.Medium,
            VideoQuality.Low
        };

        foreach (var quality in qualities)
        {
            string? url = null;

            foreach (var source in sources)
            {
                try
                {
                    url = await ParserFacade.GetMovieSrcAsync(source.PageUrl, quality);
                    if (!string.IsNullOrWhiteSpace(url))
                        break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Rezka parse failed for {source.PageUrl} [{quality}]: {ex.Message}");
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

        movie.CreatedAt = DateTime.UtcNow;
        await Uow.SaveChangesAsync(ct);
    }

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

    private async Task<Movie> UpsertFromExternalAsync(
        ExternalMovieData data,
        IReadOnlyDictionary<string, Genre> genreMap,
        CancellationToken ct)
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

        var poster = await ResolvePosterAsync(data, ct);

        var movie = new Movie
        {
            RuTitle = data.RuTitle,
            UaTitle = NullIfEmpty(data.UaTitle),
            RuInEngTitle = NullIfEmpty(data.RuInEngTitle),
            Description = NullIfEmpty(data.Description),
            Director = NullIfEmpty(data.Director) ?? "Unknown",
            ReleaseYear = data.ReleaseYear,
            PosterUrl = NullIfEmpty(poster),
            KpRating = data.KpRating,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var name in data.GenreNames.Select(Normalize).Where(n => n.Length > 0).Distinct())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(name);
            Console.ResetColor();

            if (genreMap.TryGetValue(name, out var genre))
                movie.MovieGenres.Add(new MovieGenre { Movie = movie, Genre = genre, GenreId = genre.Id });
        }

        await Uow.Movies.AddAsync(movie, ct);
        return movie;
    }

    private async Task<string?> ResolvePosterAsync(ExternalMovieData data, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(data.RuInEngTitle) && data.ReleaseYear > 0)
        {
            var poster = await Omdb.GetPosterByTitleAndYearAsync(data.RuInEngTitle, data.ReleaseYear, ct);

            if (!string.IsNullOrWhiteSpace(poster))
                return poster;
        }

        if (!string.IsNullOrWhiteSpace(data.RuTitle) && data.ReleaseYear > 0)
            return await Omdb.GetPosterByTitleAndYearAsync(data.RuTitle, data.ReleaseYear, ct);

        return null;
    }

    private static bool NeedsEnrichment(Movie m) =>
        string.IsNullOrWhiteSpace(m.PosterUrl)
        || string.IsNullOrWhiteSpace(m.Description)
        || m.KpRating <= 0;

    private static bool NeedsLinkRefresh(Movie movie) =>
        movie.CreatedAt is null
        || movie.CreatedAt == default
        || DateTime.UtcNow - movie.CreatedAt.Value >= LinkTtl;

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

        ExternalMovieData? best = null;
        var bestScore = 0;

        foreach (var c in candidates)
        {
            var apiTitle = Normalize(c.RuTitle);
            var apiAlt = Normalize(c.RuInEngTitle);
            var apiDirector = Normalize(c.Director);

            var score = 0;

            if (apiTitle == title || apiAlt == title)
                score += 100;
            else if (apiTitle.Contains(title, StringComparison.Ordinal)
                     || title.Contains(apiTitle, StringComparison.Ordinal)
                     || apiAlt.Contains(title, StringComparison.Ordinal)
                     || title.Contains(apiAlt, StringComparison.Ordinal))
                score += 50;

            if (!string.IsNullOrEmpty(director) && !string.IsNullOrEmpty(apiDirector))
            {
                if (apiDirector == director)
                    score += 80;
                else if (apiDirector.Contains(director, StringComparison.Ordinal)
                         || director.Contains(apiDirector, StringComparison.Ordinal))
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
        if (string.IsNullOrWhiteSpace(movie.PosterUrl))
        {
            var poster = await ResolvePosterAsync(data, ct);
            if (!string.IsNullOrWhiteSpace(poster))
                movie.PosterUrl = poster;
        }

        if (string.IsNullOrWhiteSpace(movie.Description)
            && !string.IsNullOrWhiteSpace(data.Description))
            movie.Description = data.Description;

        if (movie.KpRating <= 0 && data.KpRating > 0)
            movie.KpRating = data.KpRating;

        if (string.IsNullOrWhiteSpace(movie.Director)
            || string.Equals(movie.Director, "Unknown", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(data.Director)
                && !string.Equals(data.Director, "Unknown", StringComparison.OrdinalIgnoreCase))
                movie.Director = data.Director;
        }

        if (string.IsNullOrWhiteSpace(movie.UaTitle) && !string.IsNullOrWhiteSpace(data.UaTitle))
            movie.UaTitle = data.UaTitle;

        if (string.IsNullOrWhiteSpace(movie.RuInEngTitle)
            && !string.IsNullOrWhiteSpace(data.RuInEngTitle))
            movie.RuInEngTitle = data.RuInEngTitle;
    }

    private async Task ApplyGenresAsync(Movie movie, ExternalMovieData data, CancellationToken ct)
    {
        var genreMap = await LoadGenreMapAsync(ct);
        var existingIds = movie.MovieGenres.Select(mg => mg.GenreId).ToHashSet();

        foreach (var name in data.GenreNames.Select(Normalize).Where(n => n.Length > 0).Distinct())
        {
            if (genreMap.TryGetValue(name, out var genre) && existingIds.Add(genre.Id))
            {
                movie.MovieGenres.Add(new MovieGenre
                {
                    MovieId = movie.Id,
                    Movie = movie,
                    GenreId = genre.Id,
                    Genre = genre
                });
            }
        }
    }

    private async Task<Dictionary<string, Genre>> LoadGenreMapAsync(CancellationToken ct)
    {
        var genres = await Uow.Genres.ListAsync(new EmptySpecification<Genre>(), ct);
        var map = new Dictionary<string, Genre>();
        foreach (var g in genres)
            map.TryAdd(Normalize(g.Name), g);
        return map;
    }

    private static string? NullIfEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Trim().ToLowerInvariant().Replace('ё', 'е');
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