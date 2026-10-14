using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Dtos.Auth;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminSourceController(IAdminSourceService sourceService):ControllerBase
    {
        public IAdminSourceService SourceService { get; } = sourceService;

        [HttpGet("movie/{id:int}")]
        [ProducesResponseType(typeof(List<VideoSourceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<VideoSourceDto>>> GetByIdAsync(int id,CancellationToken ct)
        {
            var result = await SourceService.GetByMovieIdAsync(id, ct);
            return Ok(result);

        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddSourceAsync([FromBody] AdminSourceDto dto,CancellationToken ct)
        {
            await SourceService.AddSourceAsync(dto, ct);
            return Ok();
        }
        [HttpPut("{sourceId:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatusAsync(int sourceId,  [FromQuery] SourceStatus status,CancellationToken ct)
        {
            await SourceService.UpdateStatusAsync(sourceId, status, ct);
            return Ok();
        }
    }
}
