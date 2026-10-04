using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Interfaces.Services;

public interface IReviewService
{
    Task<List<ReviewDto>> GetByMovieIdAsync(int movieId, CancellationToken ct);
    Task CreateAsync(int userId, int movieId, string text, double rating, CancellationToken ct);
}