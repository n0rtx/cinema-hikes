using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaHikes.Infrastructure.Persistence.Seed.AdminRolesSeeding
{
    public static class AdminSeeder
    {
        private sealed record AdminChickChick(string Username, string Role, string Email, string Password);

        public static async Task SeedAsync(IServiceProvider service, CancellationToken ct)
        {
            await using var scope = service.CreateAsyncScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var admins = new List<AdminChickChick>()
                { new("admin", "Admin", "admin@gmail.com", "Admin_1234") };


            foreach (var admin in admins)
            {
                var adminUser = await userManager.FindByEmailAsync(admin.Email);
                if (adminUser == null)
                {
                    var administrator = new AppUser
                    {
                        UserName = admin.Username,
                        Email = admin.Email,
                        EmailConfirmed = true,
                        RegisteredAt = DateTime.UtcNow,
                    };
                    var result = await userManager.CreateAsync(administrator, admin.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(administrator, admin.Role);
                    }
                }
            }
        }
    }
}