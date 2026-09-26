namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record MovieListItemDto(
    int Id,
    string RuTitle,
    string UaTitle,
    string RuInEngTitle,
    string PosterUrl,
    short ReleaseYear,
    double KpRating,
    IReadOnlyList<GenreDto> Genres);