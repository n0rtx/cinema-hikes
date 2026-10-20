using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Specifications;

public class MovieSearchSpecification : BaseSpecification<Movie>
{
    public MovieSearchSpecification(string query, int? page = null, int? pageSize = null)
        : base(m => m.RuTitle.Contains(query) || m.UaTitle.Contains(query) || m.RuInEngTitle.Contains(query))
    {
        if (page.HasValue && pageSize.HasValue)
            ApplyPaging((page.Value - 1) * pageSize.Value, pageSize.Value);
    }
}