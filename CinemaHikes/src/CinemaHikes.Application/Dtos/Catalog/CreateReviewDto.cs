namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record CreateReviewDto(int MovieId, string Text, double Rating);