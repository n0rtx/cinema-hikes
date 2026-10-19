using System.Text.Json.Serialization;

namespace CinemaHikes.Infrastructure.External.Omdb;

public sealed class OmdbPosterResponse
{
    [JsonPropertyName("Poster")]
    public string? Poster { get; set; }

    [JsonPropertyName("Response")]
    public string? Response { get; set; }

    [JsonPropertyName("Error")]
    public string? Error { get; set; }
}