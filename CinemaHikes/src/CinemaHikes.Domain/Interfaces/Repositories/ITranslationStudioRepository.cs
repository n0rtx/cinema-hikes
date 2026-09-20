using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces.Repositories;

public interface ITranslationStudioRepository : IRepository<TranslationStudio>
{
    Task<TranslationStudio?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}