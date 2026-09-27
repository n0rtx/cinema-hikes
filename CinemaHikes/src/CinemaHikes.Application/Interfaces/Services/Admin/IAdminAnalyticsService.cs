using CinemaHikes.Application.Dtos.Admin;

namespace CinemaHikes.Application.Interfaces.Services.Admin;


public interface IAdminAnalyticsService
{
    Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken ct);
}