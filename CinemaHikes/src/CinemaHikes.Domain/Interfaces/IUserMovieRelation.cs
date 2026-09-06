namespace CinemaHikes.Domain.Interfaces;

public interface IUserMovieRelation
{
    int UserId { get; set; }
    
    int MovieId { get; set; }
    
    DateTime CreatedAt { get; set; }
}