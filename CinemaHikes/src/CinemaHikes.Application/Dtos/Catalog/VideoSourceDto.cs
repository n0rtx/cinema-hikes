using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record VideoSourceDto(
    int Id,
    int MovieId,
    string ProviderName,
    string PageUrl,
    short Priority,
    SourceStatus Status);