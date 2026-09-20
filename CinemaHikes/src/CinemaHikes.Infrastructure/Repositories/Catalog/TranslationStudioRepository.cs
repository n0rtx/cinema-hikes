using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

public class TranslationStudioRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<TranslationStudio>(dbContext), ITranslationStudioRepository
{
    public async Task<TranslationStudio?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await DbContext.TranslationStudios
            .FirstOrDefaultAsync(ts => ts.Name == name, cancellationToken);
}