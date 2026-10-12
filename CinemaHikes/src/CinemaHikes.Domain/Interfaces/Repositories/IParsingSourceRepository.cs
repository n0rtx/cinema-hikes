using CinemaHikes.Domain.Entities.Parsing;

namespace CinemaHikes.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for parser site configurations and scraping source settings.
/// </summary>
public interface IParsingSourceRepository : IRepository<ParsingSourceConfig>
{
    /// <summary>
    /// Retrieves all enabled parsing source configurations for background scrapers and bots.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of enabled <see cref="ParsingSourceConfig"/> entities.</returns>
    Task<List<ParsingSourceConfig>> GetEnabledAsync(CancellationToken cancellationToken = default);
}