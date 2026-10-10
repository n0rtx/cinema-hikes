using CinemaHikes.Application.Dtos.Auth;
using CinemaHikes.Application.Interfaces.Auth;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.ExternalAuth;
using CinemaHikes.Domain.Interfaces.Security;
using Microsoft.AspNetCore.Identity;

namespace CinemaHikes.Application.Services.Auth;

public sealed class AuthService(
    UserManager<AppUser> userManager,
    IJwtTokenService jwtTokenService, IGitHubOAuthClient gitHubClient) : IAuthService
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
    public async Task<AuthResponseDto> LoginWithGitHubAsync(string code,CancellationToken ct = default)
    {
        var providerMasafaka = "GitHub";
        var gh = await gitHubClient.GetUserByCodeAsync(code,ct);
        var providerKey = gh.Id.ToString();
        var user = await userManager.FindByLoginAsync(providerMasafaka, providerKey);
        if(user is null)
        {
            if(string.IsNullOrWhiteSpace(gh.Email))
            {
                throw new InvalidOperationException("Please verify GitHub account email.And try again.");

            }
            user = await userManager.FindByEmailAsync(gh.Email);
            if(user is null) // новый
            {
                user = new AppUser
                {
                    UserName = await MakeUniqueUserNameAsync(gh.Login),
                    Email = gh.Email,
                    EmailConfirmed = true,
                    RegisteredAt = DateTime.UtcNow,
                    
                };
                var create = await userManager.CreateAsync(user);
                if (!create.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", create.Errors.Select(e => e.Description)));
                }
            }
            var link = await userManager.AddLoginAsync(user, new UserLoginInfo(providerMasafaka, providerKey, providerMasafaka));
            if(!link.Succeeded)
            {
                throw new InvalidOperationException(
                       string.Join("; ", link.Errors.Select(e => e.Description)));
            }
            
        }
        var token = jwtTokenService.GenerateToken(new TokenClaimsData(user.Id, user.UserName!, user.Email!));
        return new AuthResponseDto(token, user.UserName!, user.Email!);

    }
    private async Task<string> MakeUniqueUserNameAsync(string login)
    {
        
        var baseName = login.Length < 3 ? $"gh_{login}" : login;
        if (baseName.Length > 45) baseName = baseName[..45];

        var name = baseName;
        var i = 1;
        while (await userManager.FindByNameAsync(name) is not null)
            name = $"{baseName}{i++}";
        return name;
    }
}