namespace CinemaHikes.Application.Dtos.Parsing;

public sealed record ParsingSourceConfigDto(
    int Id,
    string Name,
    string BaseUrl,
    string ParserType,
    bool IsEnabled);