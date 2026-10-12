using CinemaHikes.Domain.Interfaces.Security;

namespace CinemaHikes.Domain.Interfaces;

/// <summary>
/// Defines cryptographic token generation services for JSON Web Tokens (JWT).
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a signed JWT security token containing standard and user-specific claims.
    /// </summary>
    /// <param name="tokenClaims">The claims payload containing user identifier, name, and email.</param>
    /// <returns>A signed compact serialization JWT string.</returns>
    string GenerateToken(TokenClaimsData tokenClaims);
}