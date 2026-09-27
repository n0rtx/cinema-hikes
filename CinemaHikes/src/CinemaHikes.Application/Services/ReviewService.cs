using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services;

public sealed class ReviewService(IUnitOfWork uow) : IReviewService
{
    private IUnitOfWork Uow { get; } = uow;

    public async Task<List<ReviewDto>> GetByMovieIdAsync(int movieId, CancellationToken ct)
    {
        var reviews = await Uow.Reviews.GetByMovieIdAsync(movieId, ReviewStatus.Approved, ct);
        return reviews.Select(r => new ReviewDto(
            r.Id, r.MovieId, r.UserId, r.Text, r.Rating, r.Status, r.CreatedAt)).ToList();
    }

    public async Task CreateAsync(int userId, int movieId, string text, double rating, CancellationToken ct)
    {
        await Uow.Reviews.AddAsync(new Review
        {
            UserId = userId,
            MovieId = movieId,
            Text = text,
            Rating = rating,
            Status = ReviewStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Movie = null!
        }, ct);
        await Uow.SaveChangesAsync(ct);
    }
}