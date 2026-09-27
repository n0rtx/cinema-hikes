using System.Text.Json.Serialization;

namespace CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities.SubEntities;

public sealed class PoiskKinoNamed
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}