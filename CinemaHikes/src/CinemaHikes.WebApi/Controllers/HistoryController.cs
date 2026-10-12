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
    /// <summary>
    /// Provides HTTP endpoints for managing the authenticated user's view history and playback progress.
    /// </summary>
    /// <remarks>
    /// Allows users to resume watching where they left off and track previously watched movies.
    /// </remarks>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HistoryController(IViewHistoryService historyService):ControllerBase
    {
        /// <summary>
        /// Gets the view history service handling history tracking logic.
        /// </summary>
        public IViewHistoryService HistoryService { get; } = historyService;

        /// <summary>
        /// Retrieves the entire viewing history of the currently authenticated user.
        /// </summary>
        /// <remarks>
        /// Returns watched movies along with playback progress timestamp and last viewed date.
        /// </remarks>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A list of <see cref="ViewHistoryDto"/> representing watched movies and playback progress.</returns>
        /// <response code="200">Returns the user's viewing history.</response>
        /// <response code="401">User is unauthenticated or user ID claim is invalid.</response>
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

        /// <summary>
        /// Saves or updates the playback progress (in seconds) for a specified movie.
        /// </summary>
        /// <remarks>
        /// If an entry already exists for the user and movie, updates the timestamp and progress in seconds. Otherwise, inserts a new entry.
        /// </remarks>
        /// <param name="movieId">The unique identifier of the movie.</param>
        /// <param name="request">The progress payload containing playback position in seconds.</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="NoContentResult"/> on successful progress save.</returns>
        /// <response code="204">Progress was successfully recorded.</response>
        /// <response code="401">User is unauthenticated or user ID claim is invalid.</response>
        [HttpPut("{movieId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
