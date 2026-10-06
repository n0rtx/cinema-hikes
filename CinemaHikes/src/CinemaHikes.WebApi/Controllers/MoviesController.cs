using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    private const int MaxPageSize = 50;

    private IMovieService MovieService { get; } = movieService;

    [HttpGet]
    [ProducesResponseType(typeof(List<MovieListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MovieListItemDto>>> GetPagedAsync(
        [FromQuery] int? genreId,
        [FromQuery] int? year,
        [FromQuery] double? minRating,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var movies = await MovieService.GetPagedAsync(
            genreId, year, minRating, page, pageSize, ct);

        return Ok(movies);
    }

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

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<MovieListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<MovieListItemDto>>> SearchAsync(
        [FromQuery] string query,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return BadRequest("Query must be at least 2 characters");

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var movies = await MovieService.SearchAsync(
            query.Trim(), page, pageSize, cancellationToken);

        return Ok(movies);
    }
}