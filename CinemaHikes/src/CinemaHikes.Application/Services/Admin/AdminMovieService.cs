using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services.Admin;

/// <summary>
/// Provides administrative services for movie lifecycle operations, including creation, update, and deletion.
/// </summary>
public sealed class AdminMovieService(IUnitOfWork uow) : IAdminMovieService
{
    private IUnitOfWork Uow { get; } = uow;

    /// <summary>
    /// Creates a new movie entry and associates it with specified genres.
    /// </summary>
    /// <param name="dto">The movie creation payload containing titles, poster URL, release year, rating, and genre IDs.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The newly created movie's identifier.</returns>
    /// <exception cref="InvalidOperationException">Thrown when any referenced genre ID does not exist.</exception>
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

    /// <summary>
    /// Updates movie metadata and resets genre associations.
    /// </summary>
    /// <param name="id">The unique identifier of the movie to update.</param>
    /// <param name="dto">The updated movie payload.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <exception cref="InvalidOperationException">Thrown when the movie or any referenced genre ID is not found.</exception>
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

    /// <summary>
    /// Deletes a movie by ID from the catalog.
    /// </summary>
    /// <param name="id">The unique identifier of the movie to remove.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <exception cref="InvalidOperationException">Thrown when the movie is not found.</exception>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var movie = await Uow.Movies.GetByIdAsync(id, ct)
                    ?? throw new InvalidOperationException($"Movie {id} not found.");
        Uow.Movies.Remove(movie);
        await Uow.SaveChangesAsync(ct);
    }
}