namespace CinemaHikes.Domain.Interfaces.External;

public sealed record ExternalMovieData(
    int KpId,
    string RuTitle,
    string UaTitle,
    string RuInEngTitle,
    string Description,
    string Director,
    short ReleaseYear,
    string PosterUrl,
    double KpRating,
    IReadOnlyList<string> GenreNames);