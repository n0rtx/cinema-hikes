using System.Text.Json.Serialization;

namespace CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities;

public sealed class PoiskKinoSearchResponse
{
    [JsonPropertyName("docs")]
    public List<PoiskKinoMovieDoc> Docs { get; set; } = new();
}