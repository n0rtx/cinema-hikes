using CinemaHikes.Application.Dtos.Bot;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Interfaces.Services;

/// <summary>
/// Defines service operations for resolving downloadable video streams or direct links for movies.
/// </summary>
public interface IMovieDownloadService
{
    /// <summary>
    /// Resolves and retrieves direct video download link or cached stream for a movie at specified video quality.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="quality">The requested video quality (e.g., 360p, 480p, 720p, 1080p).</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="DownloadResultDto"/> containing direct video link or file stream metadata.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the movie is not found or has no active video sources.</exception>
    Task<DownloadResultDto> GetDownloadAsync(int movieId, VideoQuality quality, CancellationToken ct);
}