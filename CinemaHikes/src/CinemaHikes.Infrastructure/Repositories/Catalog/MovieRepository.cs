using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

public class MovieRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<Movie>(dbContext), IMovieRepository
{
    public async Task<Movie?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await DbContext.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.VideoSources)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
}