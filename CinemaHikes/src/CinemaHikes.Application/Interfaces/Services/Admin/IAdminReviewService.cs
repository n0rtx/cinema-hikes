using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Interfaces.Services.Admin
{
    public interface IAdminReviewService
    {
        Task<bool> ChangeReviewStatusAsync(ChangeReviewStatusDto changeReviewStatus,CancellationToken ct);
        Task<List<ReviewDto>> GetPagedAsync(ReviewStatus? status,int? movieId,int? userId,int  page, int pageSize, CancellationToken ct);
    }
}
