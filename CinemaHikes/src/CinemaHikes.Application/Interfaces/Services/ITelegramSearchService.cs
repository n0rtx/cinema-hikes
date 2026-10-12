using CinemaHikes.Application.Dtos.Bot;

namespace CinemaHikes.Application.Interfaces.Services;

/// <summary>
/// Defines service operations for searching movies tailored for Telegram bot inline queries and responses.
/// </summary>
public interface ITelegramSearchService
{
    /// <summary>
    /// Searches for movies matching the query text for Telegram bot display.
    /// </summary>
    /// <param name="query">The movie title query string.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="TelegramMovieResultDto"/> containing concise movie metadata for inline buttons.</returns>
    Task<List<TelegramMovieResultDto>> SearchAsync(string query, int limit, CancellationToken ct);
}