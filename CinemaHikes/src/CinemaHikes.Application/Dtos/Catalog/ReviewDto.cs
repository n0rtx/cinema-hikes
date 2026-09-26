using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record ReviewDto(
    int Id,
    int MovieId,
    int UserId,
    string Text,
    double Rating,
    ReviewStatus Status,
    DateTime CreatedAt);