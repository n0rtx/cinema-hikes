using System.Text.Json.Serialization;

namespace CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities.SubEntities;

public sealed class PoiskKinoPoster
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("previewUrl")]
    public string? PreviewUrl { get; set; }
}