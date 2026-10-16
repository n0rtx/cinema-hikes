using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHikes.Application.Dtos.Admin
{
    public sealed record ChangeReviewStatusDto(int ReviewId, bool IsAccepted, int AdminId);
}
