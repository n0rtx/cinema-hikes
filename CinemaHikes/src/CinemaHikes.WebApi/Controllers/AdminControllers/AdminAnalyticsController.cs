using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Interfaces.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminAnalyticsController(IAdminAnalyticsService analyticsService): ControllerBase
    {
        public IAdminAnalyticsService AdminAnalytics { get; } = analyticsService;

        [HttpGet]
        [ProducesResponseType(typeof(AnalyticsSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AnalyticsSummaryDto>> Get(CancellationToken ct)
        {
            var result = await AdminAnalytics.GetSummaryAsync(ct);
            return Ok(result);

        }
    }
}
