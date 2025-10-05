using System;
using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests;

public class JwtServiceTests
{
    private const string JwtKey = "test-key-that-is-long-enough-to-be-secure-123456";
    private const string JwtIssuer = "TestIssuer";

    [Fact]
    public async Task GenerateTokenPairAsync_PersistsRefreshToken()
    {
        await using var context = CreateContext();
        var service = new JwtService(context);
        var user = await SeedUserAsync(context);

        var tokens = await service.GenerateTokenPairAsync(
            user,
            JwtKey,
            JwtIssuer,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromDays(7));

        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
        Assert.True(tokens.AccessTokenExpiresUtc > DateTime.UtcNow);
        Assert.True(tokens.RefreshTokenExpiresUtc > DateTime.UtcNow);

        var storedTokens = await context.RefreshTokens.Where(rt => rt.UserId == user.Id).ToListAsync();
        Assert.Single(storedTokens);
        Assert.Null(storedTokens[0].RevokedUtc);
        Assert.Equal(tokens.RefreshTokenExpiresUtc, storedTokens[0].ExpiresUtc);
    }

    [Fact]
    public async Task RefreshTokenAsync_RotatesTokenAndRevokesPrevious()
    {
        await using var context = CreateContext();
        var service = new JwtService(context);
        var user = await SeedUserAsync(context);

        var initialTokens = await service.GenerateTokenPairAsync(
            user,
            JwtKey,
            JwtIssuer,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromDays(7));

        var refreshResult = await service.RefreshTokenAsync(
            initialTokens.RefreshToken,
            JwtKey,
            JwtIssuer,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromDays(7));

        Assert.True(refreshResult.Success);
        Assert.Null(refreshResult.Error);
        Assert.NotNull(refreshResult.Tokens);
    var refreshedTokens = refreshResult.Tokens!;

        Assert.NotEqual(initialTokens.RefreshToken, refreshedTokens.RefreshToken);

        var storedTokens = await context.RefreshTokens.Where(rt => rt.UserId == user.Id).ToListAsync();
        Assert.Equal(2, storedTokens.Count);

        var revoked = storedTokens.Single(rt => rt.RevokedUtc is not null);
        var active = storedTokens.Single(rt => rt.RevokedUtc is null);

        Assert.NotNull(revoked.ReplacedByTokenHash);
    Assert.True(revoked.RevokedUtc.HasValue);
        Assert.True(active.ExpiresUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task RefreshTokenAsync_FailsWhenTokenRevoked()
    {
        await using var context = CreateContext();
        var service = new JwtService(context);
        var user = await SeedUserAsync(context);

        var initialTokens = await service.GenerateTokenPairAsync(
            user,
            JwtKey,
            JwtIssuer,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromDays(7));

        var firstRefresh = await service.RefreshTokenAsync(
            initialTokens.RefreshToken,
            JwtKey,
            JwtIssuer,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromDays(7));

        Assert.True(firstRefresh.Success);

        var reuseAttempt = await service.RefreshTokenAsync(
            initialTokens.RefreshToken,
            JwtKey,
            JwtIssuer,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromDays(7));

        Assert.False(reuseAttempt.Success);
        Assert.Equal("Refresh token is no longer active.", reuseAttempt.Error);
        Assert.Null(reuseAttempt.Tokens);
    }

    private static async Task<User> SeedUserAsync(AppDbContext context)
    {
        var user = new User
        {
            Username = $"user_{Guid.NewGuid():N}",
            Email = $"user_{Guid.NewGuid():N}@mail.test",
            PasswordHash = "hashed-password",
            Role = UserRole.Client,
            IsActive = true
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, new NoopLifecycleService());
    }

    private sealed class NoopLifecycleService : IDbContextLifecycleService
    {
        public void UpdateTimestamps(Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker changeTracker)
        {
            // No-op for tests
        }
    }
}
