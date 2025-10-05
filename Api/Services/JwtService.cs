using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

/// <summary>
/// Represents a pair of access and refresh tokens along with their expiry metadata.
/// </summary>
public record TokenPair(
    string AccessToken,
    DateTime AccessTokenExpiresUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresUtc
);

/// <summary>
/// Service interface for issuing and managing JWT access tokens and refresh tokens.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a new access token and refresh token for the specified user.
    /// </summary>
    Task<TokenPair> GenerateTokenPairAsync(
        User user,
        string jwtKey,
        string jwtIssuer,
        TimeSpan accessTokenLifetime,
        TimeSpan refreshTokenLifetime,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rotates a refresh token and returns a new token pair if the provided refresh token is valid.
    /// </summary>
    Task<(bool Success, string? Error, TokenPair? Tokens)> RefreshTokenAsync(
        string refreshToken,
        string jwtKey,
        string jwtIssuer,
        TimeSpan accessTokenLifetime,
        TimeSpan refreshTokenLifetime,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an active refresh token for the specified user.
    /// </summary>
    Task<bool> RevokeRefreshTokenAsync(
        int userId,
        string refreshToken,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of <see cref="IJwtService"/> for issuing short-lived JWT access tokens and managing long-lived refresh tokens.
/// </summary>
public class JwtService : IJwtService
{
    private readonly AppDbContext _dbContext;

    public JwtService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TokenPair> GenerateTokenPairAsync(
        User user,
        string jwtKey,
        string jwtIssuer,
        TimeSpan accessTokenLifetime,
        TimeSpan refreshTokenLifetime,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var accessTokenExpires = now.Add(accessTokenLifetime);
        var accessToken = CreateJwtToken(user, jwtKey, jwtIssuer, accessTokenExpires);

        var refreshTokenEnvelope = CreateRefreshToken(user, refreshTokenLifetime, ipAddress);

        await RemoveExpiredTokensAsync(user.Id, cancellationToken);

        _dbContext.RefreshTokens.Add(refreshTokenEnvelope.Entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TokenPair(
            accessToken,
            accessTokenExpires,
            refreshTokenEnvelope.Token,
            refreshTokenEnvelope.Entity.ExpiresUtc);
    }

    public async Task<(bool Success, string? Error, TokenPair? Tokens)> RefreshTokenAsync(
        string refreshToken,
        string jwtKey,
        string jwtIssuer,
        TimeSpan accessTokenLifetime,
        TimeSpan refreshTokenLifetime,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeTokenHash(refreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null)
        {
            return (false, "Invalid refresh token.", null);
        }

        if (!storedToken.IsActive)
        {
            return (false, "Refresh token is no longer active.", null);
        }

        var user = storedToken.User;
        var now = DateTime.UtcNow;
        var accessTokenExpires = now.Add(accessTokenLifetime);
        var accessToken = CreateJwtToken(user, jwtKey, jwtIssuer, accessTokenExpires);

        var newRefreshEnvelope = CreateRefreshToken(user, refreshTokenLifetime, ipAddress);

        storedToken.RevokedUtc = now;
        storedToken.RevokedByIp = ipAddress;
        storedToken.ReplacedByTokenHash = newRefreshEnvelope.TokenHash;

        await RemoveExpiredTokensAsync(user.Id, cancellationToken);

        _dbContext.RefreshTokens.Add(newRefreshEnvelope.Entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, null, new TokenPair(
            accessToken,
            accessTokenExpires,
            newRefreshEnvelope.Token,
            newRefreshEnvelope.Entity.ExpiresUtc));
    }

    public async Task<bool> RevokeRefreshTokenAsync(
        int userId,
        string refreshToken,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeTokenHash(refreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.UserId == userId && rt.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            return false;
        }

        storedToken.RevokedUtc = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string CreateJwtToken(User user, string jwtKey, string jwtIssuer, DateTime expiresUtc)
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
            expires: expiresUtc,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static RefreshTokenEnvelope CreateRefreshToken(User user, TimeSpan lifetime, string? ipAddress)
    {
        var rawToken = GenerateSecureToken();
        var tokenHash = ComputeTokenHash(rawToken);
        var expiresUtc = DateTime.UtcNow.Add(lifetime);

        var entity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresUtc = expiresUtc,
            CreatedByIp = ipAddress
        };

        return new RefreshTokenEnvelope(rawToken, tokenHash, entity);
    }

    private async Task RemoveExpiredTokensAsync(int userId, CancellationToken cancellationToken)
    {
        var expiredTokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.ExpiresUtc < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        if (expiredTokens.Count == 0)
        {
            return;
        }

        _dbContext.RefreshTokens.RemoveRange(expiredTokens);
    }

    private readonly record struct RefreshTokenEnvelope(string Token, string TokenHash, RefreshToken Entity);
}