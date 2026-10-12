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

        public async Task<int> CreateAsync()
    }
}
