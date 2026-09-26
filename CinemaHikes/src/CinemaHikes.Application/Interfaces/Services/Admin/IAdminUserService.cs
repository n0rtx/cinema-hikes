namespace CinemaHikes.Application.Interfaces.Services.Admin;

public interface IAdminUserService
{
    Task<List<UserDto>> GetAllAsync(CancellationToken ct);
    Task AssignRoleAsync(int userId, string roleName, CancellationToken ct);
}