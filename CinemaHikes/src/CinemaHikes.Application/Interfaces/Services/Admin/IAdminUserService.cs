using CinemaHikes.Application.Dtos.Admin;

namespace CinemaHikes.Application.Interfaces.Services.Admin;

/// <summary>
/// Defines administrative operations for user and role management.
/// </summary>
public interface IAdminUserService
{
    /// <summary>
    /// Retrieves all registered application users with their assigned roles.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="UserDto"/> representing registered users.</returns>
    Task<List<UserDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Assigns a security role to the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roleName">The name of the role to assign (e.g., Admin, Moderator, User).</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when role assignment fails.</exception>
    Task AssignRoleAsync(AdminUserRoleDto dto, CancellationToken ct);
    Task RemoveRoleAsync(AdminUserRoleDto dto, CancellationToken ct);
}