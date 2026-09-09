using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHikes.Application.Dtos.Auth
{
    public sealed record ChangePasswordRequestDto(string OldPassword,string NewPassword);
}
