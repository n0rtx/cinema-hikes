using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaHikes.Infrastructure.Persistence.Seed;

public static class CatalogSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CinemaHikesDbContext>();

        if (await db.Movies.AnyAsync(ct))
            return;

        var lostFilm = await db.TranslationStudios
            .FirstOrDefaultAsync(s => s.Name == "LostFilm", ct);

        if (lostFilm is null)
        {
            lostFilm = new TranslationStudio { Name = "LostFilm" };
            db.TranslationStudios.Add(lostFilm);
            await db.SaveChangesAsync(ct);
        }

        var genreNames = new[]
        {
            "Боевик", "Драма", "Комедия", "Фантастика",
            "Триллер", "Криминал", "Приключения", "Фэнтези"
        };

        var genres = new Dictionary<string, Genre>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in genreNames)
        {
            var g = await db.Genres.FirstOrDefaultAsync(x => x.Name == name, ct);
            if (g is null)
            {
                g = new Genre { Name = name };
                db.Genres.Add(g);
            }

            genres[name] = g;
        }

        await db.SaveChangesAsync(ct);

        var now = DateTime.UtcNow;

        var data = new (
            string Ru, string Ua, string RuInEng, string Desc, string Dir,
            short Year, string Poster, double Rating, string LinkUrl, string[] GenreKeys)[]
            {
                (
                    "Начало", "Початок", "Nachalo",
                    "Вор, крадущий секреты через сны, получает шанс искупить вину.",
                    "Кристофер Нолан", 2010,
                    "https://m.media-amazon.com/images/I/71uKM+LdgFL.jpg", 8.7,
                    "https://nl215.werkecdn.me/s/FHssFcgnyxG8F1jm8M-AfDd0FBQUFBQUFBQUFBUnRXS0ZBUG9BSzRpV3BTdUlscVZE.GDRFJsvzjPNjnHI11bpoTqwWpSRyxcXT14ALfw/UHD_1313/Inception.2010.D.ru.mm.4K.SDR.BDRip.HUAI.2160pp_720.mp4",
                    new[] { "Фантастика", "Боевик", "Триллер" }
                ),
                (
                    "Интерстеллар", "Інтерстеллар", "Interstellar",
                    "Команда исследователей отправляется сквозь червоточину в поисках нового дома.",
                    "Кристофер Нолан", 2014,
                    "https://m.media-amazon.com/images/I/71JC2qvPx5L._AC_UF894,1000_QL80_.jpg", 8.6,
                    "https://prx5-cogent.ukrtelcdn.net/s__emerald/6217c8b18a96f19dc711f267e4ee03ab:2026100715:U05SL0dMVnlaZ012N2FlbEdyU0MzK3BlQ2xUTEJRRDdmdi9KbmM3RHJJcGN2WHowMzQ0dkEvWEJCSmhDaGNRY0NyemJyZG92Sis2ZDRzVTVtMENzbWc9PQ==/5/8/4/2/6/m1jb4.mp4",
                    new[] { "Фантастика", "Драма", "Приключения" }
                ),
                (
                    "Матрица", "Матриця", "Matritsa",
                    "Хакер узнаёт правду о реальности и вступает в борьбу с машинами.",
                    "Лана Вачовски", 1999,
                    "https://m.media-amazon.com/images/I/71PfZFFz9yL.jpg", 8.5,
                    "https://ВАШ_URL_3/720p.m3u8",
                    new[] { "Фантастика", "Боевик" }
                ),
                (
                    "Тёмный рыцарь", "Темний лицар", "Tyomnyy rytsar",
                    "Бэтмен противостоит хаосу, который несёт Джокер в Готэм.",
                    "Кристофер Нолан", 2008,
                    "https://m.media-amazon.com/images/I/81IfoBox2TL.jpg", 8.9,
                    "https://ВАШ_URL_4/720p.m3u8",
                    new[] { "Боевик", "Криминал", "Драма" }
                ),
                (
                    "Джентльмены", "Джентльмени", "Dzhentlmeny",
                    "Американский экспат в Лондоне пытается продать свою империю марихуаны.",
                    "Гай Ричи", 2019,
                    "https://m.media-amazon.com/images/I/51ZdEpgFUgL._AC_UL450_SY450_QL70_.jpg", 8.5,
                    "https://ВАШ_URL_5/720p.m3u8",
                    new[] { "Комедия", "Криминал", "Боевик" }
                ),
                (
                    "Дюна", "Дюна", "Dyuna",
                    "Пол Атрейдес отправляется на пустынную планету Арракис.",
                    "Дени Вильнёв", 2021,
                    "https://m.media-amazon.com/images/I/61Mde7eiFbL.jpg", 8.2,
                    "https://ВАШ_URL_6/720p.m3u8",
                    new[] { "Фантастика", "Приключения", "Драма" }
                ),
                (
                    "Побег из Шоушенка", "Втеча з Шоушенка", "Pobeg iz Shoushenka",
                    "Банкир, осуждённый за убийство, находит надежду за решёткой.",
                    "Фрэнк Дарабонт", 1994,
                    "https://m.media-amazon.com/images/I/911USrdQtPL.jpg", 9.1,
                    "https://ВАШ_URL_7/720p.m3u8",
                    new[] { "Драма" }
                ),
                (
                    "Форрест Гамп", "Форрест Гамп", "Forrest Gamp",
                    "История простодушного человека, ставшего свидетелем эпохи.",
                    "Роберт Земекис", 1994,
                    "https://m.media-amazon.com/images/I/91++WV6FP4L._AC_UF894,1000_QL80_.jpg", 8.9,
                    "https://ВАШ_URL_8/720p.m3u8",
                    new[] { "Драма", "Комедия" }
                ),
                (
                    "Властелин колец: Братство кольца", "Володар перснів: Братство кільця",
                    "Vlastelin kolets: Bratstvo koltsa",
                    "Хоббит отправляется уничтожить Кольцо Всевластья.",
                    "Питер Джексон", 2001,
                    "https://upload.wikimedia.org/wikipedia/ru/archive/0/08/20060425181629%21The_Lord_of_the_Rings._The_Fellowship_of_the_Ring_%E2%80%94_movie.jpg", 8.6,
                    "https://ВАШ_URL_9/720p.m3u8",
                    new[] { "Фэнтези", "Приключения" }
                ),
                (
                    "Джон Уик", "Джон Вік", "Dzhon Uik",
                    "Бывший киллер мстит за убитую собаку и украденный автомобиль.",
                    "Чад Стахелски", 2014,
                    "https://m.media-amazon.com/images/I/71i6JuSZUGL.jpg", 7.5,
                    "https://ВАШ_URL_10/720p.m3u8",
                    new[] { "Боевик", "Триллер", "Криминал" }
                ),
            };

        foreach (var item in data)
        {
            var movie = new Movie
            {
                RuTitle = item.Ru,
                UaTitle = item.Ua,
                RuInEngTitle = item.RuInEng,
                Description = item.Desc,
                Director = item.Dir,
                ReleaseYear = item.Year,
                PosterUrl = item.Poster,
                KpRating = item.Rating,
                CreatedAt = now
            };

            foreach (var gName in item.GenreKeys)
            {
                if (!genres.TryGetValue(gName, out var genre))
                    continue;

                movie.MovieGenres.Add(new MovieGenre
                {
                    Movie = movie,
                    Genre = genre
                });
            }

            movie.MovieLinks.Add(new MovieLink
            {
                Movie = movie,
                TranslationStudio = lostFilm,
                VideoQuality = VideoQuality.High,
                Url = item.LinkUrl,
                UpdatedAt = now
            });

            db.Movies.Add(movie);
        }

        await db.SaveChangesAsync(ct);
    }
}