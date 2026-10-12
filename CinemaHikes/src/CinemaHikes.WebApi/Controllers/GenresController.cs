using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers
{
    /// <summary>
    /// Provides HTTP endpoints for browsing movie genres.
    /// </summary>
    /// <remarks>
    /// Supports client-side caching of genres list to minimize database lookups.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController(IGenreService genreService) : ControllerBase
    {
        private IGenreService GenreService { get; } = genreService;

        /// <summary>
        /// Retrieves the list of all available movie genres.
        /// </summary>
        /// <remarks>
        /// The response is cached for 300 seconds (5 minutes) using HTTP Response Caching.
        /// </remarks>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A list of <see cref="GenreDto"/> containing genre IDs and names.</returns>
        /// <response code="200">Returns the complete list of genres.</response>
        [HttpGet]
        [ResponseCache(Duration = 300)]
        [ProducesResponseType(typeof(List<GenreDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GenreDto>>> GetAll(CancellationToken cancellationToken)
        {
            var genres = await GenreService.GetAllAsync(cancellationToken);
            
            return Ok(genres);
        }
    }
}