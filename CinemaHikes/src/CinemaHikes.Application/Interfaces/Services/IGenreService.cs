using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Interfaces.Services;


public interface IGenreService
{
    Task<List<GenreDto>> GetAllAsync(CancellationToken ct);
}
    
