using System.Security.Claims;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Dtos.User;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaHikes.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HistoryController(IViewHistoryService historyService):ControllerBase
    {
        public IViewHistoryService HistoryService { get; } = historyService;

        [HttpGet]
        [ProducesResponseType(typeof(List<ViewHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ViewHistoryDto>>> GetAllAsync(CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var result = await HistoryService.GetByUserIdAsync(userId, ct);
            return Ok(result);
        }
        [HttpPut("{movieId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpsertProgressAsync(
        int movieId, [FromBody] UpdateProgressRequestDto request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await HistoryService.UpsertProgressAsync(userId, movieId, request.ProgressSeconds, ct);
            return NoContent();
        }

    }
}
