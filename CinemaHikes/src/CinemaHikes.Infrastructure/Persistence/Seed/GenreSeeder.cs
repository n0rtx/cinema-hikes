using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Persistence.Seed;

public static class GenreSeeder
{
    private static readonly string[] Names =
    {
        "аниме", "биография", "боевик", "вестерн", "военный", "детектив", "детский",
        "документальный", "драма", "история", "комедия", "криминал", "мелодрама",
        "музыка", "мультфильм", "мюзикл", "приключения", "семейный", "спорт",
        "триллер", "ужасы", "фантастика", "фильм-нуар", "фэнтези"
    };

    public static async Task SeedAsync(CinemaHikesDbContext db, CancellationToken ct = default)
    {
        var existing = await db.Genres.Select(g => g.Name).ToListAsync(ct);
        var toAdd = Names
            .Where(n => !existing.Contains(n, StringComparer.OrdinalIgnoreCase))
            .Select(n => new Genre { Name = n })
            .ToList();

        if (toAdd.Count == 0) return;

        db.Genres.AddRange(toAdd);
        await db.SaveChangesAsync(ct);
    }
}