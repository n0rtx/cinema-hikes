using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services;

/// <summary>
/// Provides domain services for managing and moderating movie reviews.
/// </summary>
public sealed class ReviewService(IUnitOfWork uow) : IReviewService
{
    private IUnitOfWork Uow { get; } = uow;

    /// <summary>
    /// Retrieves all approved reviews for a given movie.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of approved <see cref="ReviewDto"/> entries.</returns>
    public async Task<List<ReviewDto>> GetByMovieIdAsync(int movieId, CancellationToken ct)
    {
        var reviews = await Uow.Reviews.GetByMovieIdAsync(movieId, ReviewStatus.Approved, ct);
        return reviews.Select(r => new ReviewDto(
            r.Id, r.MovieId, r.UserId, r.Text, r.Rating, r.Status, r.CreatedAt)).ToList();
    }

    /// <summary>
    /// Submits a new user review with Pending moderation status.
    /// </summary>
    /// <param name="userId">The unique identifier of the reviewing user.</param>
    /// <param name="movieId">The unique identifier of the movie.</param>
    /// <param name="text">The content text of the review.</param>
    /// <param name="rating">The numeric score assigned to the movie.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
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