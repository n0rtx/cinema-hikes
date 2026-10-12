using CinemaHikes.Application.Dtos.Auth;

namespace CinemaHikes.Application.Interfaces.Auth;

/// <summary>
/// Defines authentication and account management operations for application users.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user account with credentials and returns an authentication response.
    /// </summary>
    /// <param name="request">The registration data including username, email, and password.</param>
    /// <returns>An <see cref="AuthResponseDto"/> containing the generated JWT token and user details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user creation fails or username/email is taken.</exception>
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    /// <summary>
    /// Authenticates a user with email/username and password.
    /// </summary>
    /// <param name="request">The login credentials including username/email and password.</param>
    /// <returns>An <see cref="AuthResponseDto"/> containing the JWT token and user details.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Retrieves user profile information by unique user identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A <see cref="UserProfileDto"/> containing username, email, and roles.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the specified user is not found.</exception>
    Task<UserProfileDto> GetCurrentUserAsync(int userId);

    /// <summary>
    /// Changes the password for the specified user account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="request">The payload containing current password and new password.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the password change operation fails.</exception>
    Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request);

    /// <summary>
    /// Authenticates or registers a user via GitHub OAuth authorization code.
    /// </summary>
    /// <param name="code">The temporary OAuth authorization code returned by GitHub.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="AuthResponseDto"/> containing the generated JWT token and user details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when GitHub token exchange or user fetching fails.</exception>
    Task<AuthResponseDto> LoginWithGitHubAsync(string code, CancellationToken ct);
}