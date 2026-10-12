using CinemaHikes.Application.Dtos.User;

namespace CinemaHikes.Application.Interfaces.Services;


/// <summary>
/// Defines service operations for managing user's favorite movies.
/// </summary>
public interface IFavoriteService
{
    /// <summary>
    /// Retrieves all movies favorited by the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="FavoriteDto"/> representing the user's favorites.</returns>
    Task<List<FavoriteDto>> GetByUserIdAsync(int userId, CancellationToken ct);

    /// <summary>
    /// Adds a movie to the user's favorites list if not already present.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="movieId">The unique identifier of the movie to add.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddAsync(int userId, int movieId, CancellationToken ct);

    /// <summary>
    /// Removes a movie from the user's favorites list if present.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="movieId">The unique identifier of the movie to remove.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RemoveAsync(int userId, int movieId, CancellationToken ct);
}