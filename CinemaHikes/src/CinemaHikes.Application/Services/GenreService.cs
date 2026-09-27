using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Specifications;

namespace CinemaHikes.Application.Services;

public sealed class GenreService(IUnitOfWork uow) : IGenreService
{
    private IUnitOfWork Uow { get; } = uow;

    public async Task<List<GenreDto>> GetAllAsync(CancellationToken ct)
    {
        var genres = await Uow.Genres.ListAsync(new EmptySpecification<Genre>(), ct);
        return genres.Select(g => new GenreDto(g.Id, g.Name)).ToList();
    }
}