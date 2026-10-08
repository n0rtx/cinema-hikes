using System.Security.Claims;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    [HttpGet("movie/{movieId:int}")]
    [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewDto>>> GetByMovieAsync(
        int movieId,
        CancellationToken ct)
    {
        var reviews = await reviewService.GetByMovieIdAsync(movieId, ct);
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateReviewDto dto,
        CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await reviewService.CreateAsync(userId, dto.MovieId, dto.Text, dto.Rating, ct);
        return StatusCode(StatusCodes.Status201Created);
    }
}