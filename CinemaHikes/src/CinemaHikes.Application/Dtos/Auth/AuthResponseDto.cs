namespace CinemaHikes.Application.Dtos.Auth;

public sealed record AuthResponseDto(string Token, string Username, string Email);