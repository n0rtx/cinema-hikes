using System.Security.Claims;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Dtos.User;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers
{
    /// <summary>
    /// Provides HTTP endpoints for managing the authenticated user's favorite movies collection.
    /// </summary>
    /// <remarks>
    /// All endpoints require user authentication via Bearer JWT token.
    /// </remarks>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FavoritesController(IFavoriteService favoriteService) : ControllerBase
    {
        /// <summary>
        /// Gets the favorite service handling business operations.
        /// </summary>
        public IFavoriteService FavoriteService { get; } = favoriteService;

        /// <summary>
        /// Retrieves the list of all movies marked as favorites by the current user.
        /// </summary>
        /// <remarks>
        /// Resolves the user identity from token claims and fetches their saved favorite movies.
        /// </remarks>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A list of <see cref="FavoriteDto"/> representing user's favorite movies.</returns>
        /// <response code="200">Returns the user's favorite movies list.</response>
        /// <response code="401">User is unauthenticated or user ID claim is invalid.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<FavoriteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<FavoriteDto>>> GetAllAsync(CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var result = await FavoriteService.GetByUserIdAsync(userId,ct);
            return Ok(result);
        }

        /// <summary>
        /// Adds a movie to the authenticated user's favorites list.
        /// </summary>
        /// <remarks>
        /// Creates a relation record linking the specified user and movie if it does not already exist.
        /// </remarks>
        /// <param name="movieId">The unique identifier of the movie to add.</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="NoContentResult"/> on successful addition.</returns>
        /// <response code="204">Movie was successfully added to favorites.</response>
        /// <response code="401">User is unauthenticated.</response>
        [HttpPost("{movieId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddAsync(int movieId,CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            await  FavoriteService.AddAsync(userId, movieId, ct);
            return NoContent();
        }

        /// <summary>
        /// Removes a movie from the authenticated user's favorites list.
        /// </summary>
        /// <remarks>
        /// Deletes the relation record between the user and the movie if found.
        /// </remarks>
        /// <param name="movieId">The unique identifier of the movie to remove.</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="NoContentResult"/> on successful removal.</returns>
        /// <response code="204">Movie was successfully removed from favorites.</response>
        /// <response code="401">User is unauthenticated.</response>
        [HttpDelete("{movieId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveAsync(int movieId, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            await FavoriteService.RemoveAsync(userId, movieId, ct);
            return NoContent();
        }
    }
}
