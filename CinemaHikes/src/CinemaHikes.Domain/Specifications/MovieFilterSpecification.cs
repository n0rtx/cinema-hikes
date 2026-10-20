using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Specifications;

public sealed class MovieFilterSpecification : BaseSpecification<Movie>
{
    public MovieFilterSpecification(int? genreId, int? year, double? minRating, int? page = null, int? pageSize = null)
    {
        AddCriteria(m =>
            (!genreId.HasValue || m.MovieGenres.Any(mg => mg.GenreId == genreId)) &&
            (!year.HasValue || m.ReleaseYear == year) &&
            (!minRating.HasValue || m.KpRating >= minRating));

        AddInclude("MovieGenres.Genre");

        ApplyOrderByDescending(m => m.KpRating);
        AddThenBy(m => m.Id);

        if (page.HasValue && pageSize.HasValue)
            ApplyPaging((page.Value - 1) * pageSize.Value, pageSize.Value);
    }
}