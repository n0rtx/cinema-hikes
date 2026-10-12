using CinemaHikes.Application.Dtos.User;

namespace CinemaHikes.Application.Interfaces.Services;

/// <summary>
/// Defines service operations for tracking and querying user movie viewing history and playback progress.
/// </summary>
public interface IViewHistoryService
{
    /// <summary>
    /// Retrieves the playback history for a specified user, ordered by most recently watched.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="ViewHistoryDto"/> containing playback positions and movie metadata.</returns>
    Task<List<ViewHistoryDto>> GetByUserIdAsync(int userId, CancellationToken ct);

    /// <summary>
    /// Updates or inserts viewing progress in seconds for the user and movie.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="progressSeconds">The current video playback timestamp in seconds.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpsertProgressAsync(int userId, int movieId, int progressSeconds, CancellationToken ct);
}