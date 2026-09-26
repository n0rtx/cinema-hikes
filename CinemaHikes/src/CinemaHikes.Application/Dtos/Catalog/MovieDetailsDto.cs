namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record MovieDetailsDto(
    int Id,
    string RuTitle,
    string UaTitle,
    string RuInEngTitle,
    string Description,
    string Director,
    short ReleaseYear,
    string PosterUrl,
    double KpRating,
    DateTime CreatedAt,
    IReadOnlyList<GenreDto> Genres,
    IReadOnlyList<VideoSourceDto> VideoSources,
    IReadOnlyList<MovieLinkDto> MovieLinks,
    IReadOnlyList<ReviewDto> Reviews);