using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories.Catalog;

/// <summary>
/// Entity Framework Core implementation of <see cref="IGenreRepository"/> for movie genres.
/// </summary>
public class GenreRepository(CinemaHikesDbContext dbContext)
    : BaseRepository<Genre>(dbContext), IGenreRepository
{
    /// <summary>
    /// Finds a genre entity matching the specified name.
    /// </summary>
    /// <param name="name">The name of the genre.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching <see cref="Genre"/> entity, or <c>null</c> if none matched.</returns>
    public async Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await DbContext.Genres
            .FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
}