using CinemaHikes.Domain.Interfaces.External;
using CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities;

namespace CinemaHikes.Infrastructure.External.PoiskKino;

public static class PoiskKinoMapper
{
    public static ExternalMovieData Map(PoiskKinoMovieDoc d)
    {
        var director = d.Persons?
            .FirstOrDefault(p =>
                string.Equals(p.Profession, "director", StringComparison.OrdinalIgnoreCase)
                || string.Equals(p.EnProfession, "director", StringComparison.OrdinalIgnoreCase))
            ?.Name ?? "Unknown";

        var genres = d.Genres?
            .Select(g => g.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToList() ?? new List<string>();

        var ru = d.Name ?? d.AlternativeName ?? d.EnName ?? "Untitled";
        var eng = d.AlternativeName ?? d.EnName ?? ru;

        return new ExternalMovieData(
            d.Id,
            ru,
            ru,
            eng,
            d.Description ?? string.Empty,
            director,
            (short)(d.Year ?? 0),
            d.Poster?.Url ?? d.Poster?.PreviewUrl ?? string.Empty,
            d.Rating?.Kp ?? 0,
            genres);
    }
}