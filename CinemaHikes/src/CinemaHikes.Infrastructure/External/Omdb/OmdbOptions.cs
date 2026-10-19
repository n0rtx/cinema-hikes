namespace CinemaHikes.Infrastructure.External.Omdb;

public sealed class OmdbOptions
{
    public const string SectionName = "Omdb";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://www.omdbapi.com/";
}