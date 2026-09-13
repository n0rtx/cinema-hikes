namespace CinemaHikes.Application.Dtos.Auth;

public sealed record ChangePasswordRequestDto(string OldPassword, string NewPassword);