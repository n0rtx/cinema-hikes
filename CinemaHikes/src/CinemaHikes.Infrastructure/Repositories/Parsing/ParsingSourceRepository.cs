using CinemaHikes.Domain.Entities.Parsing;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Parsing;

/// <summary>
/// Entity Framework Core implementation of <see cref="IParsingSourceRepository"/> for parsing source configurations.
/// </summary>
public class ParsingSourceRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<ParsingSourceConfig>(dbContext), IParsingSourceRepository
{
    /// <summary>
    /// Retrieves all enabled parsing configurations where IsEnabled is true.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of enabled <see cref="ParsingSourceConfig"/> entities.</returns>
    public async Task<List<ParsingSourceConfig>> GetEnabledAsync(CancellationToken cancellationToken = default)
        => await DbContext.ParsingSourceConfigs
            .Where(p => p.IsEnabled)
            .ToListAsync(cancellationToken);
}