using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Models;

public interface IJwtService
{
    string GenerateToken(User user, string jwtKey, string jwtIssuer);
}

public class JwtService : IJwtService
{
    public string GenerateToken(User user, string jwtKey, string jwtIssuer)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userid", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}