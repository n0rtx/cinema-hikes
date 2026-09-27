using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

public class ReviewRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<Review>(dbContext), IReviewRepository
{
    public async Task<List<Review>> GetByMovieIdAsync(
        int movieId, ReviewStatus? status = null, CancellationToken cancellationToken = default)
        => await DbContext.Reviews
            .Where(r => r.MovieId == movieId && (!status.HasValue || r.Status == status))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
}