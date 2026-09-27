using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record MovieLinkDto(
    int Id,
    int MovieId,
    TranslationStudioDto TranslationStudio,
    VideoQuality VideoQuality,
    string Url,
    DateTime UpdatedAt);