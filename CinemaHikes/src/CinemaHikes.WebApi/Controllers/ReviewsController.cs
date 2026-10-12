using System.Security.Claims;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers;

/// <summary>
/// Provides HTTP endpoints for browsing and posting movie reviews and ratings.
/// </summary>
/// <remarks>
/// Allows any user to read reviews for a movie, while submitting a review requires authentication.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    /// <summary>
    /// Retrieves all approved reviews and ratings for a specified movie.
    /// </summary>
    /// <remarks>
    /// Returns the text, numeric rating, submission timestamp, and author details for the movie's reviews.
    /// </remarks>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="ReviewDto"/> representing reviews for the movie.</returns>
    /// <response code="200">Returns the list of reviews.</response>
    [HttpGet("movie/{movieId:int}")]
    [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewDto>>> GetByMovieAsync(
        int movieId,
        CancellationToken ct)
    {
        var reviews = await reviewService.GetByMovieIdAsync(movieId, ct);
        return Ok(reviews);
    }

    /// <summary>
    /// Creates a new review and rating for a movie on behalf of the authenticated user.
    /// </summary>
    /// <remarks>
    /// Extracts the user ID from token claims and submits the review content and rating to the review service.
    /// </remarks>
    /// <param name="dto">The review payload containing movie ID, review text, and numeric rating.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A status code 201 Created on successful review creation.</returns>
    /// <response code="201">The review was successfully created.</response>
    /// <response code="401">User is unauthenticated or user ID claim is invalid.</response>
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