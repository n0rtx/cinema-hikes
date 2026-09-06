using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Entities.Catalog;

public sealed class MovieLink
{
    public int Id { get; set; }

    public int MovieId { get; set; }
    public required Movie Movie { get; set; }

    public int TranslationStudioId { get; set; }
    public required TranslationStudio TranslationStudio { get; set; }

    public required VideoQuality VideoQuality { get; set; }

    public required string Url { get; set; }

    public required DateTime UpdatedAt { get; set; }
}