using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Interfaces.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    Task<List<Review>> GetByMovieIdAsync(
        int movieId, ReviewStatus? status = null, CancellationToken cancellationToken = default);
}