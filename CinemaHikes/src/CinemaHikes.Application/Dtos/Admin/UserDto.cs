namespace CinemaHikes.Application.Dtos.Admin;

public sealed record UserDto(int Id, string Username, string Email, IReadOnlyList<string> Roles);