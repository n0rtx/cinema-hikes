using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Dtos.User;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services;

public sealed class FavoriteService(IUnitOfWork uow) : IFavoriteService
{
    private IUnitOfWork Uow { get; } = uow;

    public async Task<List<FavoriteDto>> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        var items = await Uow.FavoriteMovies.GetByUserIdAsync(userId, ct);
        return items.Select(f => new FavoriteDto(
            f.Id, f.UserId, f.MovieId, MapMovie(f.Movie), f.CreatedAt)).ToList();
    }

    public async Task AddAsync(int userId, int movieId, CancellationToken ct)
    {
        if (await Uow.FavoriteMovies.FindAsync(userId, movieId, ct) is not null)
            return;

        await Uow.FavoriteMovies.AddAsync(new FavoriteMovie
        {
            UserId = userId,
            MovieId = movieId,
            CreatedAt = DateTime.UtcNow,
            Movie = null!
        }, ct);
        await Uow.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(int userId, int movieId, CancellationToken ct)
    {
        var existing = await Uow.FavoriteMovies.FindAsync(userId, movieId, ct);
        if (existing is null)
            return;

        Uow.FavoriteMovies.Remove(existing);
        await Uow.SaveChangesAsync(ct);
    }

    private static MovieListItemDto MapMovie(Movie m) => new(
        m.Id, m.RuTitle, m.UaTitle, m.RuInEngTitle, m.PosterUrl, m.ReleaseYear, m.KpRating,
        m.MovieGenres.Select(mg => new GenreDto(mg.Genre.Id, mg.Genre.Name)).ToList());
}