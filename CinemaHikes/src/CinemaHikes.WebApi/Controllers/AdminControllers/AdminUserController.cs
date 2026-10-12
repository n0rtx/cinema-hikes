using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Dtos.User;
using CinemaHikes.Application.Interfaces.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController(IAdminUserService admUserService): ControllerBase
    {
        public IAdminUserService AdminUserService { get; } = admUserService;

        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<UserDto>>> GetAll(CancellationToken ct)
        {

            var users = await AdminUserService.GetAllAsync(ct);
            return Ok(users);
        }
    }
}
