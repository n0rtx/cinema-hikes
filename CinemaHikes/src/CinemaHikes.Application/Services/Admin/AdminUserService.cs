using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace CinemaHikes.Application.Services.Admin;

public sealed class AdminUserService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<int>> roleManager) : IAdminUserService
{
    public async Task<List<UserDto>> GetAllAsync(CancellationToken ct)
    {
        var users = userManager.Users.ToList();
        var result = new List<UserDto>(users.Count);

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(new UserDto(
                user.Id,
                user.UserName!,
                user.Email!,
                roles.ToList()));
        }

        return result;
    }

    public async Task AssignRoleAsync(int userId, string roleName, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
                   ?? throw new InvalidOperationException($"User {userId} not found.");

        if (!await roleManager.RoleExistsAsync(roleName))
            throw new InvalidOperationException($"Role '{roleName}' does not exist.");

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
            await userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}