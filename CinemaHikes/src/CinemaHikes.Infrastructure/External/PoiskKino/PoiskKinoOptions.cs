namespace CinemaHikes.Infrastructure.External.PoiskKino;

public sealed class PoiskKinoOptions
{
    public const string SectionName = "PoiskKino";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.poiskkino.dev/";
}