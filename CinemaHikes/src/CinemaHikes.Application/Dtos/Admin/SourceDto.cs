using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHikes.Application.Dtos.Admin
{
    public sealed record AdminSourceDto(int MovieId, string ProviderName, string PageUrl, short Priority);
}
