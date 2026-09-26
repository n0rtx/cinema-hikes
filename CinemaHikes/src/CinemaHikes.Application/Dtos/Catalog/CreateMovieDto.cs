namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record CreateMovieDto(
    string RuTitle,
    string UaTitle,
    string RuInEngTitle,
    string Description,
    string Director,
    short ReleaseYear,
    string PosterUrl,
    double KpRating,
    IReadOnlyList<int> GenreIds);