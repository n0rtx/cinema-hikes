namespace CinemaHikes.Application.Dtos.Auth;

public sealed record RegisterRequestDto(string Username, string Email, string Password);