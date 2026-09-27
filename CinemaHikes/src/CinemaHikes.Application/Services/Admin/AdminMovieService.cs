using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services.Admin;

public sealed class AdminMovieService(IUnitOfWork uow) : IAdminMovieService
{
    private IUnitOfWork Uow { get; } = uow;

    public async Task<int> CreateAsync(CreateMovieDto dto, CancellationToken ct)
    {
        var movie = new Movie
        {
            RuTitle = dto.RuTitle,
            UaTitle = dto.UaTitle,
            RuInEngTitle = dto.RuInEngTitle,
            Description = dto.Description,
            Director = dto.Director,
            ReleaseYear = dto.ReleaseYear,
            PosterUrl = dto.PosterUrl,
            KpRating = dto.KpRating,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var genreId in dto.GenreIds)
        {
            var genre = await Uow.Genres.GetByIdAsync(genreId, ct)
                        ?? throw new InvalidOperationException($"Genre {genreId} not found.");
            movie.MovieGenres.Add(new MovieGenre { Genre = genre, Movie = movie, GenreId = genreId });
        }

        await Uow.Movies.AddAsync(movie, ct);
        await Uow.SaveChangesAsync(ct);
        return movie.Id;
    }

    public async Task UpdateAsync(int id, UpdateMovieDto dto, CancellationToken ct)
    {
        var movie = await Uow.Movies.GetWithDetailsAsync(id, ct)
                    ?? throw new InvalidOperationException($"Movie {id} not found.");

        movie.RuTitle = dto.RuTitle;
        movie.UaTitle = dto.UaTitle;
        movie.RuInEngTitle = dto.RuInEngTitle;
        movie.Description = dto.Description;
        movie.Director = dto.Director;
        movie.ReleaseYear = dto.ReleaseYear;
        movie.PosterUrl = dto.PosterUrl;
        movie.KpRating = dto.KpRating;
        movie.MovieGenres.Clear();

        foreach (var genreId in dto.GenreIds)
        {
            var genre = await Uow.Genres.GetByIdAsync(genreId, ct)
                        ?? throw new InvalidOperationException($"Genre {genreId} not found.");
            movie.MovieGenres.Add(new MovieGenre
            {
                Genre = genre,
                Movie = movie,
                GenreId = genreId,
                MovieId = id
            });
        }

        Uow.Movies.Update(movie);
        await Uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var movie = await Uow.Movies.GetByIdAsync(id, ct)
                    ?? throw new InvalidOperationException($"Movie {id} not found.");
        Uow.Movies.Remove(movie);
        await Uow.SaveChangesAsync(ct);
    }
}