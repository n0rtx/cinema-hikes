using System.Security.Claims;
using CinemaHikes.Domain.Interfaces.Security;

namespace CinemaHikes.Domain.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(TokenClaimsData tokenClaims);
}