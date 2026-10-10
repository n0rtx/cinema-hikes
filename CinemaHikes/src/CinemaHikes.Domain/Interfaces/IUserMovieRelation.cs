using CinemaHikes.Domain.Entities.Catalog;

namespace CinemaHikes.Domain.Interfaces;

public interface IUserMovieRelation
{
    int UserId { get; set; }
    
    int MovieId { get; set; }
    Movie Movie { get; set; }
    DateTime CreatedAt { get; set; }
}