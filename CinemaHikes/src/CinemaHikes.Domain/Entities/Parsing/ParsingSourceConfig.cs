namespace CinemaHikes.Domain.Entities.Parsing;

public sealed class ParsingSourceConfig
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string BaseUrl { get; set; }
    
    public required string ParserType { get; set; }

    public bool IsEnabled { get; set; } = true;
}