namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record CreateVideoSourceDto(
    int MovieId,
    string ProviderName,
    string PageUrl,
    short Priority);