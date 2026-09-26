using CinemaHikes.Application.Dtos.User;

namespace CinemaHikes.Application.Interfaces.Services;

public interface IViewHistoryService
{
    Task<List<ViewHistoryDto>> GetByUserIdAsync(int userId, CancellationToken ct);
    Task UpsertProgressAsync(int userId, int movieId, int progressSeconds, CancellationToken ct);
}