using CinemaHikes.Application.Dtos.Admin;

namespace CinemaHikes.Application.Interfaces.Services.Admin;


/// <summary>
/// Defines administrative analytics operations for computing platform statistics.
/// </summary>
public interface IAdminAnalyticsService
{
    /// <summary>
    /// Computes summary metrics including total movies, reviews count, and viewing statistics.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="AnalyticsSummaryDto"/> with high-level platform usage totals.</returns>
    Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken ct);
}