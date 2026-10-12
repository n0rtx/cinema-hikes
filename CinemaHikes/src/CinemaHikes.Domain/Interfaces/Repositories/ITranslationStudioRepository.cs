using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for voiceover/translation studio catalog data.
/// </summary>
public interface ITranslationStudioRepository : IRepository<TranslationStudio>
{
    /// <summary>
    /// Finds a translation studio by its unique name string.
    /// </summary>
    /// <param name="name">The name of the translation studio.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching <see cref="TranslationStudio"/> entity, or <c>null</c> if not found.</returns>
    Task<TranslationStudio?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}