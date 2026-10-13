using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Interfaces.Services.Admin;

/// <summary>
/// Defines administrative operations for managing video sources and providers for movies.
/// </summary>
public interface IAdminSourceService
{
    /// <summary>
    /// Retrieves all video streaming sources registered for a given movie.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="VideoSourceDto"/> associated with the movie.</returns>
    Task<List<VideoSourceDto>> GetByMovieIdAsync(int movieId, CancellationToken ct);

    /// <summary>
    /// Adds a new video source link for a movie.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="providerName">The name of the video provider (e.g., Rezka, HDRezka).</param>
    /// <param name="pageUrl">The full URL to the video page on the provider site.</param>
    /// <param name="priority">The resolution priority order for this source.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddSourceAsync(AdminSourceDto dto, CancellationToken ct);

    /// <summary>
    /// Updates the health/operational status of a video source (e.g., Active, Dead, Checking).
    /// </summary>
    /// <param name="sourceId">The unique identifier of the video source.</param>
    /// <param name="status">The updated <see cref="SourceStatus"/> status value.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the source is not found.</exception>
    Task UpdateStatusAsync(int sourceId, SourceStatus status, CancellationToken ct);
}