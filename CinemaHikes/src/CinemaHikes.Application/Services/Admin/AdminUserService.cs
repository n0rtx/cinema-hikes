using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CinemaHikes.Application.Services.Admin;

/// <summary>
/// Provides administrative user and role assignment services using ASP.NET Core Identity.
/// </summary>
public sealed class AdminUserService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<int>> roleManager, IUserwithRolesReadRepository userwithRolesRead) : IAdminUserService
{
    /// <summary>
    /// Retrieves all registered users in the system alongside their assigned roles.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="UserDto"/> representing each user with their roles.</returns>
    public async Task<List<UserDto>> GetAllAsync(CancellationToken ct)
    {
        var users = await userwithRolesRead.GetAllUsersWithRolesAsync(ct);
        return users.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Replaces the current roles of a user with the specified role.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roleName">The name of the target role.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <exception cref="InvalidOperationException">Thrown when user does not exist, role does not exist, or assignment fails.</exception>
    public async Task AssignRoleAsync(AdminUserRoleDto dto, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(dto.UserId.ToString())
                   ?? throw new InvalidOperationException($"User {dto.UserId} not found.");

        if (!await roleManager.RoleExistsAsync(dto.RoleToAdd))
            throw new InvalidOperationException($"Role '{dto.RoleToAdd}' does not exist.");

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
            await userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await userManager.AddToRoleAsync(user, dto.RoleToAdd);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));
    }
    public async Task RemoveRoleAsync(AdminUserRoleDto dto,CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(dto.UserId.ToString()) ?? throw new InvalidOperationException($"User {dto.UserId} not found."); ;
        if (!await roleManager.RoleExistsAsync(dto.RoleToAdd))
            throw new InvalidOperationException($"Role '{dto.RoleToAdd}' does not exist.");
        var currentRoles = await userManager.GetRolesAsync(user);
        if(!currentRoles.Contains(dto.RoleToAdd))
        {
            throw new InvalidOperationException($"Role '{dto.RoleToAdd}' does not have user.");
        }
        var result = await userManager.RemoveFromRoleAsync(user, dto.RoleToAdd);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));
    }
    private static UserDto MapToDto(UserWithRoles dto) => new (dto.Id, dto.Username, dto.Email, dto.Roles, dto.AmountOfHats);
}