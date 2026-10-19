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

        var localTitle = d.Name ?? d.AlternativeName ?? d.EnName ?? "Untitled";
        
        var engCandidate = FirstNonEmpty(d.EnName, d.AlternativeName);
        var engTitle = LooksLikeEnglish(engCandidate) ? engCandidate! : localTitle;

        return new ExternalMovieData(
            d.Id,
            localTitle,
            localTitle,
            engTitle,
            d.Description ?? string.Empty,
            director,
            (short)(d.Year ?? 0),
            string.Empty,
            d.Rating?.Kp ?? 0,
            genres);
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var v in values)
        {
            if (!string.IsNullOrWhiteSpace(v))
                return v.Trim();
        }
        return null;
    }

    private static bool LooksLikeEnglish(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        return value.Any(c => c is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'));
    }
}