namespace CinemaHikes.Application.Dtos.Auth;

public sealed record UserProfileDto(int Id, string Username, string Email, DateTime CreatedAt);