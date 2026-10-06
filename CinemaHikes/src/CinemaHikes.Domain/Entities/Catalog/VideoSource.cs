using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Entities.Parsing;

namespace CinemaHikes.Domain.Entities.Catalog;

public sealed class VideoSource
{
    public int Id { get; set; }

    public required int MovieId { get; set; }
    public required Movie Movie { get; set; }

    public int? ParsingSourceConfigId { get; set; }
    public ParsingSourceConfig? ParsingSourceConfig { get; set; }

    public required string ProviderName { get; set; }

    public required string PageUrl { get; set; }

    public required short Priority { get; set; }

    public required SourceStatus Status { get; set; }
}