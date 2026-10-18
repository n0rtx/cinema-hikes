using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Domain.Entities.Users;

namespace CinemaHikes.Domain.Interfaces.Repositories
{
    public interface IUserwithRolesReadRepository
    {
        Task<List<UserWithRoles>> GetAllUsersWithRolesAsync(CancellationToken ct);
    }
}
