using CinemaHikes.Application.Dtos.Bot;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Interfaces.Services;

public interface IMovieDownloadService
{
    Task<DownloadResultDto> GetDownloadAsync(int movieId, VideoQuality quality, CancellationToken ct);
}