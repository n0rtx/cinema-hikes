using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHikes.Application.Dtos.Admin
{
    public sealed record AddRoleDto(int UserId, string RoleToAdd);
}
