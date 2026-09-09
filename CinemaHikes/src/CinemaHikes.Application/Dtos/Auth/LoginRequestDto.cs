using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHikes.Application.Dtos.Auth
{
    public sealed record LoginRequestDto(string Email,string Password);
}
