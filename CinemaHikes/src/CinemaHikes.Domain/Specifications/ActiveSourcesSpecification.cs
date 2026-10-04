using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Specifications;

public class ActiveSourcesSpecification : BaseSpecification<Movie>
{
    public ActiveSourcesSpecification()
        : base(m => m.VideoSources.Count(vs => vs.Status == SourceStatus.Active) >= 1)
    {
    }
}