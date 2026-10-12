using CinemaHikes.Application.Dtos.Catalog;

namespace CinemaHikes.Application.Interfaces.Services.Admin;

/// <summary>
/// Defines administrative catalog management operations for creating, updating, and deleting movies.
/// </summary>
public interface IAdminMovieService
{
    /// <summary>
    /// Creates a new movie entry in the catalog.
    /// </summary>
    /// <param name="dto">The movie creation payload containing titles, poster URL, release year, rating, and genre IDs.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The newly generated movie identifier.</returns>
    Task<int> CreateAsync(CreateMovieDto dto, CancellationToken ct);

    /// <summary>
    /// Updates details of an existing movie.
    /// </summary>
    /// <param name="id">The unique identifier of the movie to update.</param>
    /// <param name="dto">The updated movie payload.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the movie with the given ID is not found.</exception>
    Task UpdateAsync(int id, UpdateMovieDto dto, CancellationToken ct);

    /// <summary>
    /// Removes a movie from the catalog.
    /// </summary>
    /// <param name="id">The unique identifier of the movie to delete.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the movie with the given ID is not found.</exception>
    Task DeleteAsync(int id, CancellationToken ct);
}