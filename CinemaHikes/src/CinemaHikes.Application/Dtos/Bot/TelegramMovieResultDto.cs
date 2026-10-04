namespace CinemaHikes.Application.Dtos.Bot;

public sealed record TelegramMovieResultDto(
    int Id,
    string Title,
    short ReleaseYear,
    string? PosterUrl,
    double KpRating);