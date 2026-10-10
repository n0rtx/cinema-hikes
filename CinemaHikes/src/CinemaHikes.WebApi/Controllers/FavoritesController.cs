using System.Security.Claims;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Dtos.User;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FavoritesController(IFavoriteService favoriteService) : ControllerBase
    {
        public IFavoriteService FavoriteService { get; } = favoriteService;

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
        [HttpPost("movieId:{int}")]
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
        [HttpDelete("movieId:{int}")]
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
