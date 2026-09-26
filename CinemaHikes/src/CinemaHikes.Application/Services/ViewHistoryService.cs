using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Dtos.User;
using CinemaHikes.Application.Interfaces.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services;

public sealed class ViewHistoryService(IUnitOfWork uow) : IViewHistoryService
{
    private IUnitOfWork Uow { get; } = uow;

    public async Task<List<ViewHistoryDto>> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        var items = await Uow.ViewHistory.GetByUserIdAsync(userId, ct);
        return items.Select(h => new ViewHistoryDto(
            h.Id, h.UserId, h.MovieId, MapMovie(h.Movie), h.ProgressSeconds, h.CreatedAt)).ToList();
    }

    public async Task UpsertProgressAsync(int userId, int movieId, int progressSeconds, CancellationToken ct)
    {
        var existing = await Uow.ViewHistory.FindAsync(userId, movieId, ct);
        if (existing is null)
        {
            await Uow.ViewHistory.AddAsync(new ViewHistoryEntry
            {
                UserId = userId,
                MovieId = movieId,
                ProgressSeconds = progressSeconds,
                CreatedAt = DateTime.UtcNow,
                Movie = null!
            }, ct);
        }
        else
        {
            existing.ProgressSeconds = progressSeconds;
        }

        await Uow.SaveChangesAsync(ct);
    }

    private static MovieListItemDto MapMovie(Movie m) => new(
        m.Id, m.RuTitle, m.UaTitle, m.RuInEngTitle, m.PosterUrl, m.ReleaseYear, m.KpRating,
        m.MovieGenres.Select(mg => new GenreDto(mg.Genre.Id, mg.Genre.Name)).ToList());
}