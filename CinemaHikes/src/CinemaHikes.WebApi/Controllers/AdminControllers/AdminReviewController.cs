using CinemaHikes.Application.Dtos.Admin;
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
    public class AdminReviewController(IAdminReviewService reviewStatusService):ControllerBase
    {
        public IAdminReviewService ReviewStatusService { get; } = reviewStatusService;
        private const int MaxPageSize = 20;
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool),StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ChangeStatusAsync([FromBody] ChangeReviewStatusDto changeReviewStatusDto, CancellationToken ct)
        {
            bool result = await ReviewStatusService.ChangeReviewStatusAsync(changeReviewStatusDto, ct);
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReviewDto>>> GetPagedAsync([FromQuery]ReviewStatus? status, [FromQuery] int? movieId, [FromQuery] int? userId, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
            var reviews = await ReviewStatusService.GetPagedAsync(status, movieId, userId, page, pageSize, ct);
            return Ok(reviews);
        }

    }
}
