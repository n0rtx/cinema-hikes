using CinemaHikes.Application.Dtos.User;

namespace CinemaHikes.Application.Interfaces.Services;


public interface IFavoriteService
{
    Task<List<FavoriteDto>> GetByUserIdAsync(int userId, CancellationToken ct);
    Task AddAsync(int userId, int movieId, CancellationToken ct);
    Task RemoveAsync(int userId, int movieId, CancellationToken ct);
}