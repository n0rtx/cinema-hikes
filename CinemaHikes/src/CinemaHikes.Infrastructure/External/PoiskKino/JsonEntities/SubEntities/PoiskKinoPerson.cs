using System.Text.Json.Serialization;

namespace CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities.SubEntities;

public sealed class PoiskKinoPerson
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("profession")]
    public string? Profession { get; set; }

    [JsonPropertyName("enProfession")]
    public string? EnProfession { get; set; }
}