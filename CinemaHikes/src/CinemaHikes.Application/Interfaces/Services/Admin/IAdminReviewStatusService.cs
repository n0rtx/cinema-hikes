using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Application.Dtos.Admin;

namespace CinemaHikes.Application.Interfaces.Services.Admin
{
    public interface IAdminReviewStatusService
    {
        Task<bool> ChangeReviewStatus(ChangeReviewStatusDto changeReviewStatus,CancellationToken ct);
    }
}
