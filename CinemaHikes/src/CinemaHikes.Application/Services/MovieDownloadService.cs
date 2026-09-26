using CinemaHikes.Application.Dtos.Bot;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Bot;

namespace CinemaHikes.Application.Services;

public sealed class MovieDownloadService(
    IUnitOfWork uow,
    IMovieParserFacade parser,
    IVideoSizeChecker sizeChecker) : IMovieDownloadService
{
    private IUnitOfWork Uow { get; } = uow;
    private IMovieParserFacade Parser { get; } = parser;
    private IVideoSizeChecker SizeChecker { get; } = sizeChecker;

    public async Task<DownloadResultDto> GetDownloadAsync(int movieId, VideoQuality quality, CancellationToken ct)
    {
        var cached = await Uow.MovieLinks.GetByMovieAndQualityAsync(movieId, quality, 1, ct);
        if (cached is not null && !string.IsNullOrWhiteSpace(cached.Url))
            return await ToResultAsync(cached.Url, ct);

        var sources = (await Uow.VideoSources.GetByMovieIdAsync(movieId, ct))
            .Where(s => s.Status == SourceStatus.Active)
            .OrderBy(s => s.Priority)
            .ToList();

        if (sources.Count == 0)
            throw new InvalidOperationException($"No active sources for movie {movieId}.");

        string? url = null;
        foreach (var source in sources)
        {
            try
            {
                url = await Parser.GetMovieSrcAsync(source.PageUrl, quality);
                if (!string.IsNullOrWhiteSpace(url))
                    break;
            }
            catch
            {
            }
        }

        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException($"Could not resolve stream for movie {movieId}.");

        var studio = await Uow.TranslationStudios.GetByNameAsync("Default", ct)
                     ?? new TranslationStudio { Name = "Default" };

        if (studio.Id == 0)
            await Uow.TranslationStudios.AddAsync(studio, ct);

        await Uow.MovieLinks.UpsertAsync(new MovieLink
        {
            MovieId = movieId,
            TranslationStudioId = studio.Id == 0 ? 1 : studio.Id,
            VideoQuality = quality,
            Url = url,
            UpdatedAt = DateTime.UtcNow,
            Movie = null!,
            TranslationStudio = studio
        }, ct);
        await Uow.SaveChangesAsync(ct);

        return await ToResultAsync(url, ct);
    }

    private async Task<DownloadResultDto> ToResultAsync(string url, CancellationToken ct)
    {
        var within = await SizeChecker.IsWithinTelegramLimitAsync(url, ct);
        return new DownloadResultDto(url);
    }
}