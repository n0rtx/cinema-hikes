using CinemaHikes.Domain.Entities.Parsing;

namespace CinemaHikes.Domain.Interfaces.Repositories;

public interface IParsingSourceRepository : IRepository<ParsingSourceConfig>
{
    Task<List<ParsingSourceConfig>> GetEnabledAsync(CancellationToken cancellationToken = default);
}