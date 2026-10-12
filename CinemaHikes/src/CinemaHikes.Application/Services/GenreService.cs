using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Specifications;

namespace CinemaHikes.Application.Services;

/// <summary>
/// Provides domain operations for retrieving genre classifications.
/// </summary>
public sealed class GenreService(IUnitOfWork uow) : IGenreService
{
    private IUnitOfWork Uow { get; } = uow;

    /// <summary>
    /// Retrieves all catalog genres as DTOs.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="GenreDto"/> with genre ID and localized name.</returns>
    public async Task<List<GenreDto>> GetAllAsync(CancellationToken ct)
    {
        var genres = await Uow.Genres.ListAsync(new EmptySpecification<Genre>(), ct);
        return genres.Select(g => new GenreDto(g.Id, g.Name)).ToList();
    }
}