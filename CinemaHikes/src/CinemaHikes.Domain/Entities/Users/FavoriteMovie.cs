using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Domain.Entities.Users;

public sealed class FavoriteMovie : IUserMovieRelation
{
    public int Id { get; set; }

    public required int UserId { get; set; }

    public required int MovieId { get; set; }
    public required Movie Movie { get; set; }

    public required DateTime CreatedAt { get; set; }
}