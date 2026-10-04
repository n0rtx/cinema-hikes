namespace CinemaHikes.Domain.Interfaces.Security;

public record TokenClaimsData(int UserId, string UserName, string Email);