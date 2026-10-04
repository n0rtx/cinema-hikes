using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Dtos.User;

public sealed record ViewHistoryDto(
    int Id,
    int UserId,
    int MovieId,
    MovieListItemDto Movie,
    int ProgressSeconds,
    DateTime CreatedAt);