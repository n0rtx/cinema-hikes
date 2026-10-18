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
    public sealed class UserWithRolesReadRepository(CinemaHikesDbContext db):IUserwithRolesReadRepository
    {
        public async Task<List<UserWithRoles>> GetAllUsersWithRolesAsync(CancellationToken ct)
        {
            var users = await db.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.UserName, u.Email })
                .ToListAsync(ct);

            var userRoles = await (
                from ur in db.UserRoles
                join r in db.Roles on ur.RoleId equals r.Id
                select new { ur.UserId, RoleName = r.Name }
            ).ToListAsync(ct);

            var rolesByUser = userRoles
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.RoleName!).ToList());

            return users.Select(u => new UserWithRoles(
                u.Id,
                u.UserName!,
                u.Email!,
                rolesByUser.GetValueOrDefault(u.Id, Array.Empty<string>())
            )).ToList();
        }
    }
}
