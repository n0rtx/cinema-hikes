using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHikes.Application.Dtos.Auth
{
    public sealed record AuthResponseDto(string Token,string Username,string Email);
}
