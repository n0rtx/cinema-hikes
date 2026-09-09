using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Application.Dtos.Auth;

namespace CinemaHikes.Domain.Interfaces.AuthService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserProfileDto> GetCurrentUserAsync();
        Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    }
}
