using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController(IGenreService genreService) : ControllerBase
    {
        private IGenreService GenreService { get; } = genreService;

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