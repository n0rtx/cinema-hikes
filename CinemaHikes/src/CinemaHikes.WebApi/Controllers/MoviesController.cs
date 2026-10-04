using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class MoviesController:ControllerBase
    {
        private readonly IMovieService _movieService;
        private const int MaxPageSize = 50;
        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<MovieListItemDto>),StatusCodes.Status200OK)]
        public async Task<ActionResult<List<MovieListItemDto>>> GetPaged([FromQuery] int? genreId,
            [FromQuery] int? year, [FromQuery] double? minRating, [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,CancellationToken ct = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
            var movies = await _movieService.GetPagedAsync(genreId, year, minRating, page,pageSize,ct);
            return Ok(movies);
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieDetailsDto>> GetDetails(int id,CancellationToken cancellationToken)
        {
            var movie = await _movieService.GetDetailsAsync(id, cancellationToken);
            return movie is null ? NotFound() : Ok(movie);
        }
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<MovieListItemDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<MovieListItemDto>>> Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            {
                return BadRequest("Query must be atleast with 2 charachters");
            }
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(page,1, MaxPageSize);
            var movies = await _movieService.SearchAsync(query, page, pageSize, cancellationToken);
            return Ok(movies);
        }
    }
}
