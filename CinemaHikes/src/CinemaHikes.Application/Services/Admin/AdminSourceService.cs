using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services.Admin;

/// <summary>
/// Provides administrative management services for video streaming sources and provider links.
/// </summary>
public sealed class AdminSourceService(IUnitOfWork uow) : IAdminSourceService
{
    private IUnitOfWork Uow { get; } = uow;

    /// <summary>
    /// Retrieves all video sources configured for a specific movie.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="VideoSourceDto"/> records for the movie.</returns>
    public async Task<List<VideoSourceDto>> GetByMovieIdAsync(int movieId, CancellationToken ct)
    {
        var list = await Uow.VideoSources.GetByMovieIdAsync(movieId, ct);
        return list.Select(s => new VideoSourceDto(
            s.Id, s.MovieId, s.ProviderName, s.PageUrl, s.Priority, s.Status)).ToList();
    }

    /// <summary>
    /// Registers a new active video streaming source for a movie.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="providerName">The name of the video hosting/streaming provider.</param>
    /// <param name="pageUrl">The URL of the video page on the external provider.</param>
    /// <param name="priority">The resolution priority (lower values tried first).</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    public async Task AddSourceAsync(
        AdminSourceDto dto, CancellationToken ct)
    {
        await Uow.VideoSources.AddAsync(new VideoSource
        {
            MovieId = dto.MovieId,
            ProviderName = dto.ProviderName,
            PageUrl = dto.PageUrl,
            Priority = dto.Priority,
            Status = SourceStatus.Active,
            Movie = null!
        }, ct);
        await Uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Updates the operational status of an existing video source.
    /// </summary>
    /// <param name="sourceId">The unique identifier of the video source.</param>
    /// <param name="status">The new status to apply (Active, Dead, Checking).</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <exception cref="InvalidOperationException">Thrown when the source is not found.</exception>
    public async Task UpdateStatusAsync(int sourceId, SourceStatus status, CancellationToken ct)
    {
        var source = await Uow.VideoSources.GetByIdAsync(sourceId, ct)
                     ?? throw new InvalidOperationException($"Source {sourceId} not found.");
        source.Status = status;
        Uow.VideoSources.Update(source);
        await Uow.SaveChangesAsync(ct);
    }
}