using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Specifications;

public sealed class MovieFilterSpecification : BaseSpecification<Movie>
{
    public MovieFilterSpecification(
        IReadOnlyCollection<int>? genreIds,
        int? year,
        double? minRating,
        string? sort = null,
        bool matchAllGenres = false,
        int? page = null,
        int? pageSize = null,
        bool onlyWithPoster = false)
    {
        var ids = genreIds?.Distinct().ToArray() ?? Array.Empty<int>();
        var hasGenres = ids.Length > 0;
        var idsCount = ids.Length;

        AddCriteria(m =>
            (!hasGenres
             || (matchAllGenres
                 ? m.MovieGenres.Count(mg => ids.Contains(mg.GenreId)) == idsCount
                 : m.MovieGenres.Any(mg => ids.Contains(mg.GenreId)))) &&
            (!year.HasValue || m.ReleaseYear == year) &&
            (!minRating.HasValue || m.KpRating >= minRating) &&
            (!onlyWithPoster || (m.PosterUrl != null && m.PosterUrl != "")));

        AddInclude("MovieGenres.Genre");

        switch (sort)
        {
            case "year":
                ApplyOrderByDescending(m => m.ReleaseYear);
                break;
            case "title":
                ApplyOrderBy(m => m.RuTitle);
                break;
            default:
                ApplyOrderByDescending(m => m.KpRating);
                break;
        }
        AddThenBy(m => m.Id);

        if (page.HasValue && pageSize.HasValue)
            ApplyPaging((page.Value - 1) * pageSize.Value, pageSize.Value);
    }
}