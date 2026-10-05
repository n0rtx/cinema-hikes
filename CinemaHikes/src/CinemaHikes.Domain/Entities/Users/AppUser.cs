using Microsoft.AspNetCore.Identity;

namespace CinemaHikes.Domain.Entities.Users;

public sealed class AppUser : IdentityUser<int>
{
    public required DateTime RegisteredAt { get; set; }

    public int AmountOfHats { get; set; } = 0;
}