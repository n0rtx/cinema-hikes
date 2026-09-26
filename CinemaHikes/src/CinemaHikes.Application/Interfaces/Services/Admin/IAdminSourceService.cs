using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Interfaces.Services.Admin;

public interface IAdminSourceService
{
    Task<List<VideoSourceDto>> GetByMovieIdAsync(int movieId, CancellationToken ct);
    Task AddSourceAsync(int movieId, string providerName, string pageUrl, short priority, CancellationToken ct);
    Task UpdateStatusAsync(int sourceId, SourceStatus status, CancellationToken ct);
}