using Microsoft.AspNetCore.Identity;

namespace CinemaHikes.Infrastructure.Identity;

public sealed class AppUser : IdentityUser<int>
{
    public required DateTime RegisteredAt { get; set; }

    public int AmountOfHats { get; set; } = 0;
}