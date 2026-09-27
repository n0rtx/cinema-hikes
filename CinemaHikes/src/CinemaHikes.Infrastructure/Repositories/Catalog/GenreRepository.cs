using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

public class GenreRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<Genre>(dbContext), IGenreRepository
{
    public async Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await DbContext.Genres
            .FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
}