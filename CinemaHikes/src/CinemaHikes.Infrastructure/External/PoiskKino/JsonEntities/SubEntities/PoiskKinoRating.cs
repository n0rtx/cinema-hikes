using System.Text.Json.Serialization;

namespace CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities.SubEntities;

public sealed class PoiskKinoRating
{
    [JsonPropertyName("kp")]
    public double? Kp { get; set; }
}