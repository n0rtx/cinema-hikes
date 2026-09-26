using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Dtos.User;

public sealed record FavoriteDto(
    int Id,
    int UserId,
    int MovieId,
    MovieListItemDto Movie,
    DateTime CreatedAt);