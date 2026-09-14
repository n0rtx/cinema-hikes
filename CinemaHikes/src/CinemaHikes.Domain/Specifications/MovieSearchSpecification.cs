using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Specifications;

public class MovieSearchSpecification : BaseSpecification<Movie>
{
    public MovieSearchSpecification(string query, int page, int pageSize)
        : base(m => m.RuTitle.Contains(query) || m.UaTitle.Contains(query) || m.RuInEngTitle.Contains(query))
    {
        ApplyPaging((page - 1) * pageSize, pageSize);
    }
}