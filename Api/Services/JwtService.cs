using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Models;

/// <summary>
/// Service interface for generating JWT tokens.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <param name="jwtKey">The secret key used for signing the token.</param>
    /// <param name="jwtIssuer">The issuer of the token.</param>
    /// <returns>A signed JWT token as a string.</returns>
    string GenerateToken(User user, string jwtKey, string jwtIssuer);
}

/// <summary>
/// Implementation of IJwtService for creating JWT tokens.
/// </summary>
public class JwtService : IJwtService
{
    /// <summary>
    /// Generates a JWT token containing user claims.
    /// </summary>
    public string GenerateToken(User user, string jwtKey, string jwtIssuer)
    {
        // Create the security key and signing credentials
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Define claims to include in the token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userid", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // Create the JWT token with claims and expiration
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7), // TODO: Change to 30 mins and implement refresh token mechanism
            signingCredentials: credentials
        );

        // Return the serialized JWT token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}