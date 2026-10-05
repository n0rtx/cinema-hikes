using CinemaHikes.Application.Dtos.Auth;
using CinemaHikes.Application.Interfaces.Auth;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Security;
using Microsoft.AspNetCore.Identity;

namespace CinemaHikes.Application.Services.Auth;

public sealed class AuthService(
    UserManager<AppUser> userManager,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var user = new AppUser
        {
            UserName = request.Username,
            Email = request.Email,
            RegisteredAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        var token = jwtTokenService.GenerateToken(
            new TokenClaimsData(user.Id, user.UserName!, user.Email!));

        return new AuthResponseDto(token, user.UserName!, user.Email!);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
                   ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = jwtTokenService.GenerateToken(
            new TokenClaimsData(user.Id, user.UserName!, user.Email!));

        return new AuthResponseDto(token, user.UserName!, user.Email!);
    }

    public async Task<UserProfileDto> GetCurrentUserAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
                   ?? throw new InvalidOperationException($"User {userId} not found.");

        return new UserProfileDto(
            user.Id,
            user.UserName!,
            user.Email!,
            user.RegisteredAt);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
                   ?? throw new InvalidOperationException($"User {userId} not found.");

        var result = await userManager.ChangePasswordAsync(
            user, request.OldPassword, request.NewPassword);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}