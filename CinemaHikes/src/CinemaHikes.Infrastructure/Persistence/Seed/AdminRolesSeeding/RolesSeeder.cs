using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaHikes.Infrastructure.Persistence.Seed.AdminRolesSeeding
{
    public static class RolesSeeder
    {
        public static async Task SeedAsync(IServiceProvider service,CancellationToken ct)
        {
            await using var scope = service.CreateAsyncScope();
            var roleManager = scope.ServiceProvider.GetRequiredService <RoleManager<IdentityRole<int>>>();
            var roles = new[] { "Admin", "Moderator", "User" };
            foreach(var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
           
            
        }
    }
}
