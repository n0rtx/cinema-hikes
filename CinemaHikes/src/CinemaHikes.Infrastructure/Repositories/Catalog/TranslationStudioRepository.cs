using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

/// <summary>
/// Entity Framework Core implementation of <see cref="ITranslationStudioRepository"/> for voiceover/translation studios.
/// </summary>
public class TranslationStudioRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<TranslationStudio>(dbContext), ITranslationStudioRepository
{
    /// <summary>
    /// Finds a translation studio entity matching the given name string.
    /// </summary>
    /// <param name="name">The name of the translation studio.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching <see cref="TranslationStudio"/> entity, or <c>null</c> if none matched.</returns>
    public async Task<TranslationStudio?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await DbContext.TranslationStudios
            .FirstOrDefaultAsync(ts => ts.Name == name, cancellationToken);
}