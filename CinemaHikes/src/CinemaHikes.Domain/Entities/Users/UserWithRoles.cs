using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHikes.Domain.Entities.Users
{
    public sealed record UserWithRoles(int Id, string Username, string Email, IReadOnlyList<string> Roles, int AmountOfHats);
}
