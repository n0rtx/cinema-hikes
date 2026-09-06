using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Entities.Catalog;

public sealed class Review
{
    public int Id { get; set; }

    public required int MovieId { get; set; }
    public required Movie Movie { get; set; }

    public required int UserId { get; set; }

    public required string Text { get; set; }

    public required double Rating { get; set; }

    public required ReviewStatus Status { get; set; }

    public required DateTime CreatedAt { get; set; }
}