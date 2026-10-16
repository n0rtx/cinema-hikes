using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Interfaces.Services.Admin;
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool),StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ChangeStatusAsync([FromBody] ChangeReviewStatusDto changeReviewStatusDto, CancellationToken ct)
        {
            bool result = await ReviewStatusService.ChangeReviewStatusAsync(changeReviewStatusDto, ct);
            return Ok(result);
        }

    }
}
