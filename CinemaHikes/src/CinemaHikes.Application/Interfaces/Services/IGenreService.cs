using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Interfaces.Services;


/// <summary>
/// Defines service operations for retrieving movie genres.
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Retrieves all available genres ordered alphabetically.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="GenreDto"/> containing genre identifiers and names.</returns>
    Task<List<GenreDto>> GetAllAsync(CancellationToken ct);
}
    
