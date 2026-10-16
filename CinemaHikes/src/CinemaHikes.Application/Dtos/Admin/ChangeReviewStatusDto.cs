using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Application.Dtos.Admin
{
    public sealed record ChangeReviewStatusDto(int ReviewId, ReviewStatus newStatus);
}
