using System.Security.Claims;
using CinemaHikes.Application.Dtos.Auth;
using CinemaHikes.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHikes.WebApi.Controllers;

/// <summary>
/// Provides HTTP endpoints for user authentication, registration, profile retrieval, and OAuth workflows.
/// </summary>
/// <remarks>
/// Handles both standard username/password authentication flow and external GitHub OAuth authentication.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user account with an email, username, and password.
    /// </summary>
    /// <remarks>
    /// Validates user registration data, creates the account in the system, and generates an authentication token.
    /// </remarks>
    /// <param name="request">The registration payload containing email, user credentials, and password.</param>
    /// <returns>An <see cref="AuthResponseDto"/> containing the generated JWT token and user details.</returns>
    /// <response code="200">Returns the authentication response with JWT token.</response>
    /// <response code="400">Returned when validation fails or username/email is already registered.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> RegisterAsync(
        [FromBody] RegisterRequestDto request)
    {
        var result = await authService.RegisterAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Authenticates an existing user using their email/username and password.
    /// </summary>
    /// <remarks>
    /// Validates provided credentials against the user database and returns a signed JWT upon success.
    /// </remarks>
    /// <param name="request">The login payload containing username/email and password.</param>
    /// <returns>An <see cref="AuthResponseDto"/> containing the JWT token and basic user details.</returns>
    /// <response code="200">Credentials are valid; returns JWT token and user info.</response>
    /// <response code="401">Invalid username/email or password provided.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync(
        [FromBody] LoginRequestDto request)
    {
        var result = await authService.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the profile details of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Extracts the user ID claim (<see cref="ClaimTypes.NameIdentifier"/>) from the current JWT token context.
    /// </remarks>
    /// <returns>A <see cref="UserProfileDto"/> containing current user profile details.</returns>
    /// <response code="200">Returns the profile of the authenticated user.</response>
    /// <response code="401">User is not authenticated or the ID claim is missing/invalid.</response>
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

    /// <summary>
    /// Changes the password of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Verifies the current password before applying the new password. Requires an authenticated session.
    /// </remarks>
    /// <param name="request">The request payload containing old password and new password.</param>
    /// <returns>A <see cref="NoContentResult"/> on successful password change.</returns>
    /// <response code="204">Password was successfully changed.</response>
    /// <response code="400">Old password is incorrect or new password does not meet complexity requirements.</response>
    /// <response code="401">User is not authenticated.</response>
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

    /// <summary>
    /// Authenticates or registers a user via GitHub OAuth authorization code.
    /// </summary>
    /// <remarks>
    /// Exchanges the temporary GitHub OAuth code for an access token, fetches the user's GitHub profile,
    /// and logs in or provisions a local account.
    /// </remarks>
    /// <param name="dto">The GitHub login payload containing the OAuth authorization code.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="AuthResponseDto"/> containing the generated JWT token.</returns>
    /// <response code="200">OAuth authentication succeeded; returns JWT token.</response>
    /// <response code="400">Failed to exchange code or fetch user details from GitHub.</response>
    [HttpPost("github")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Github(GitHubLoginRequestDto dto, CancellationToken ct) => Ok(await authService.LoginWithGitHubAsync(dto.Code, ct));
}