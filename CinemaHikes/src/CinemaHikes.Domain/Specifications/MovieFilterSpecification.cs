using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Specifications;

public sealed class MovieFilterSpecification : BaseSpecification<Movie>
{
    public MovieFilterSpecification(int? genreId, int? year, double? minRating, int page, int pageSize)
    {
        AddCriteria(m => 
            (!genreId.HasValue || m.MovieGenres.Any(mg => mg.GenreId == genreId)) && 
            (!year.HasValue || m.ReleaseYear == year) &&
            (!minRating.HasValue || m.KpRating >= minRating));
        
        AddInclude(m => m.MovieGenres);
        
        ApplyOrderByDescending(m => m.KpRating);
        
        ApplyPaging((page - 1) * pageSize, pageSize);
    }
}