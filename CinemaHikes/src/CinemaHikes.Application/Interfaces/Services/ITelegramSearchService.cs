using CinemaHikes.Application.Dtos.Bot;

namespace CinemaHikes.Application.Interfaces.Services;

public interface ITelegramSearchService
{
    Task<List<TelegramMovieResultDto>> SearchAsync(string query, int limit, CancellationToken ct);
}