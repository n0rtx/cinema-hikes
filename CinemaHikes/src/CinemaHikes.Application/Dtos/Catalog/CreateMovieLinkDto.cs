using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record CreateMovieLinkDto(
    int MovieId,
    int TranslationStudioId,
    VideoQuality VideoQuality,
    string Url);