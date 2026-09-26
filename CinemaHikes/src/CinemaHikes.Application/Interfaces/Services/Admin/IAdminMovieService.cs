namespace CinemaHikes.Application.Interfaces.Services.Admin;

public interface IAdminMovieService
{
    Task<int> CreateAsync(MovieCreateDto dto, CancellationToken ct);
    Task UpdateAsync(int id, MovieCreateDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}