using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Interfaces.Services.Admin;

public interface IAdminMovieService
{
    Task<int> CreateAsync(CreateMovieDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateMovieDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}