using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminMovieController(IAdminMovieService movieService): ControllerBase
    {
        public IAdminMovieService MovieService { get; } = movieService;

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult<int>> CreateAsync([FromBody] CreateMovieDto dto,CancellationToken ct)
        {
            int newMovieId = await MovieService.CreateAsync(dto, ct);
            return Ok(newMovieId);
        }
        [HttpPost("{movieId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(int movieId, [FromBody] UpdateMovieDto dto,CancellationToken ct)
        {
            await MovieService.UpdateAsync(movieId, dto, ct);
            return Ok();
        }
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(int movieId,CancellationToken ct)
        {
            await MovieService.DeleteAsync(movieId, ct);
            return Ok();
        }
    }
}
