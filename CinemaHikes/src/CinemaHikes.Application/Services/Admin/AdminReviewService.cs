using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Specifications;

namespace CinemaHikes.Application.Services.Admin
{
    public class AdminReviewService(IUnitOfWork uow) :IAdminReviewService
    {
        public IUnitOfWork uow { get; } = uow;
        public async Task<bool> ChangeReviewStatusAsync(ChangeReviewStatusDto dto,CancellationToken ct)
        {
            var reviewToChange = 
                await uow.Reviews.GetByIdAsync(dto.ReviewId, ct)
                ?? throw new ArgumentException($"Review with id:{dto.ReviewId} not found");
            if (reviewToChange.Status == dto.newStatus) throw new ArgumentException($"Review with id {dto.ReviewId} has already status: {dto.newStatus}");
            reviewToChange.Status = dto.newStatus;
            await uow.SaveChangesAsync(ct);
            return true;
            
        }
        public async Task<List<ReviewDto>> GetPagedAsync(ReviewStatus? status, int? movieId, int? userId, int page, int pageSize, CancellationToken ct)
        {
            var specification = new ReviewFilterSpecification(status, movieId, userId, page, pageSize);
            var reviews = await uow.Reviews.ListAsync(specification, ct);
            return reviews.Select(MapReviews).ToList();

        }
        private static ReviewDto MapReviews(Review r) => new(r.Id, r.MovieId, r.UserId, r.Text, r.Rating, r.Status, r.CreatedAt);
    }
}
