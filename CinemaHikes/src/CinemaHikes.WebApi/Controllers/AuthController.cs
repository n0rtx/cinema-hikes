using System.Security.Claims;
using CinemaHikes.Application.Dtos.Auth;
using CinemaHikes.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> RegisterAsync(
        [FromBody] RegisterRequestDto request)
    {
        var result = await authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync(
        [FromBody] LoginRequestDto request)
    {
        var result = await authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUserAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var profile = await authService.GetCurrentUserAsync(userId);
        return Ok(profile);
    }

    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequestDto request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await authService.ChangePasswordAsync(userId, request);
        return NoContent();
    }
    [HttpPost("github")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Github(GitHubLoginRequestDto dto, CancellationToken ct) => Ok(await authService.LoginWithGitHubAsync(dto.Code, ct));
}