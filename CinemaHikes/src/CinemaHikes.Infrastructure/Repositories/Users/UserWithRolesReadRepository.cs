using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Users
{
    public sealed class UserWithRolesReadRepository(CinemaHikesDbContext db) : IUserwithRolesReadRepository
    {
        public async Task<List<UserWithRoles>> GetAllUsersWithRolesAsync(CancellationToken ct)
        {
            var users = await db.Users.AsNoTracking().Select(u => new
            {
                u.Id, u.UserName, u.Email,
                Roles = (from ur in db.UserRoles
                    join r in db.Roles on ur.RoleId equals r.Id
                    where ur.UserId == u.Id
                    select r.Name!).ToList()
            }).ToListAsync(ct);
            
            return users.Select(x => new UserWithRoles(x.Id, x.UserName!, x.Email!, x.Roles)).ToList();
        }
    }
}