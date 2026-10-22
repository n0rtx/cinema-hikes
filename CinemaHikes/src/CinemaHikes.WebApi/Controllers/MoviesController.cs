using System.Security.Claims;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers;

/// <summary>
/// Provides HTTP endpoints for browsing, retrieving, and searching movies in the catalog.
/// </summary>
/// <remarks>
/// Exposes public access to movie listings with support for filtering by genre, release year, minimum rating, and text search.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    private const int MaxPageSize = 50;

    private IMovieService MovieService { get; } = movieService;

    /// <summary>
    /// Retrieves a paginated list of movies based on optional genre, year, and rating filters.
    /// </summary>
    /// <remarks>
    /// The page size is clamped between 1 and 50 items. Movies are filtered using database specifications.
    /// </remarks>
    /// <param name="genreId">Optional identifier of the genre to filter movies by.</param>
    /// <param name="year">Optional release year to filter movies by.</param>
    /// <param name="minRating">Optional minimum rating threshold for filtering.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <param name="page">The 1-based page index (defaults to 1).</param>
    /// <param name="pageSize">The number of items to retrieve per page (defaults to 20, max 50).</param>
    /// <param name="onlyWithPoster">Display only movies with the poster</param>
    /// <returns>A paginated result of <see cref="MovieListItemDto"/> matching the specified filter criteria.</returns>
    /// <response code="200">Returns the filtered and paginated movie list.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<MovieListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<MovieListItemDto>>> GetPagedAsync(
        [FromQuery] int? genreId,
        [FromQuery] int? year,
        [FromQuery] double? minRating,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool onlyWithPoster = false)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var result = await MovieService.GetPagedAsync(
            genreId, year, minRating, page, pageSize, onlyWithPoster, ct);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves comprehensive details of a specific movie by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Includes movie metadata, associated genres, video sources, reviews, and available translation studios.
    /// </remarks>
    /// <param name="id">The unique identifier of the movie.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="MovieDetailsDto"/> if found, or 404 NotFound if no movie exists with the given ID.</returns>
    /// <response code="200">The movie was found and its details are returned.</response>
    /// <response code="404">No movie exists with the specified ID.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailsDto>> GetDetailsAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var movie = await MovieService.GetDetailsAsync(id, cancellationToken);
        return movie is null ? NotFound() : Ok(movie);
    }

    /// <summary>
    /// Searches for movies by title query, looking up locally first and querying external metadata provider if not found.
    /// </summary>
    /// <remarks>
    /// The search term must be at least 2 non-whitespace characters long. External search results are automatically persisted locally.
    /// </remarks>
    /// <param name="query">The text search query for movie title.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <param name="page">The 1-based page index (defaults to 1).</param>
    /// <param name="pageSize">The number of items per page (defaults to 20, max 50).</param>
    /// <returns>A paginated result of <see cref="MovieListItemDto"/> matching the query.</returns>
    /// <response code="200">Returns matching movies.</response>
    /// <response code="400">The query is null, empty, or shorter than 2 characters.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResultDto<MovieListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResultDto<MovieListItemDto>>> SearchAsync(
        [FromQuery] string query,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return BadRequest("Query must be at least 2 characters");

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        int? userId = null;
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out var parsedId))
            userId = parsedId;

        var result = await MovieService.SearchAsync(
            query.Trim(), page, pageSize, userId, cancellationToken);

        return Ok(result);
    }
}