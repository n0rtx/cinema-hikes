namespace CinemaHikes.Application.Dtos.Catalog;

public sealed record PagedResultDto<T>(IReadOnlyList<T> Items, int Total);