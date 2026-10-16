using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services.Admin
{
    public class AdminReviewService(IUnitOfWork uow) :IAdminReviewStatusService
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
    }
}
