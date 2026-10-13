using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Formats.Cbor;

namespace CinemaHikes.Infrastructure.Security;

/// <summary>
/// Implements JWT token generation with HMAC-SHA256 signing using application configuration settings.
/// </summary>
public class JwtTokenService(IConfiguration config) : IJwtTokenService
{
    /// <summary>
    /// Issues a JWT token signed with HMAC-SHA256 and valid for 7 days.
    /// </summary>
    /// <param name="tokenClaims">The claims payload with user identifier, username, and email.</param>
    /// <returns>The serialized JWT string.</returns>
    public string GenerateToken(TokenClaimsData tokenClaims)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, tokenClaims.UserId.ToString()),
            new(ClaimTypes.Name, tokenClaims.UserName),
            new(ClaimTypes.Email, tokenClaims.Email)
        };
        claims.AddRange(tokenClaims.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
   
}