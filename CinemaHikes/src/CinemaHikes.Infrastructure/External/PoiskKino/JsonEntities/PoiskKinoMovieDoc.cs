using System.Text.Json.Serialization;
using CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities.SubEntities;

namespace CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities;

public sealed class PoiskKinoMovieDoc
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("alternativeName")]
    public string? AlternativeName { get; set; }

    [JsonPropertyName("enName")]
    public string? EnName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("rating")]
    public PoiskKinoRating? Rating { get; set; }

    [JsonPropertyName("poster")]
    public PoiskKinoPoster? Poster { get; set; }

    [JsonPropertyName("genres")]
    public List<PoiskKinoNamed>? Genres { get; set; }

    [JsonPropertyName("persons")]
    public List<PoiskKinoPerson>? Persons { get; set; }
}