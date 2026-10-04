using CinemaHikes.Domain.Entities.Parsing;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Parsing;

public class ParsingSourceRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<ParsingSourceConfig>(dbContext), IParsingSourceRepository
{
    public async Task<List<ParsingSourceConfig>> GetEnabledAsync(CancellationToken cancellationToken = default)
        => await DbContext.ParsingSourceConfigs
            .Where(p => p.IsEnabled)
            .ToListAsync(cancellationToken);
}