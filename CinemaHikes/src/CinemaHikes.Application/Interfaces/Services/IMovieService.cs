using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Interfaces.Services;


public interface IMovieService
{
    Task<List<MovieListItemDto>> GetPagedAsync(
        int? genreId, int? year, double? minRating, int page, int pageSize, CancellationToken ct);

    Task<List<MovieListItemDto>> SearchAsync(string query, int page, int pageSize, CancellationToken ct);

    Task<MovieDetailsDto?> GetDetailsAsync(int id, CancellationToken ct);
}