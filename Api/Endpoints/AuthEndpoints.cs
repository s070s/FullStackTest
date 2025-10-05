using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.Dtos;
using Api.Services;
using Api.Repositories.UnitOfWork;
using System.Linq;
using Microsoft.AspNetCore.Http;


namespace Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            // Use default authorization (JWT)
            #region Register
            app.MapPost("/register", async (
                IValidationService validator,
                RegisterUserDto dto,
                AppDbContext db,
                IConfiguration config,
                IUnitOfWork unitOfWork
            ) =>
            {
                if (!validator.IsValidEmail(dto.Email))
                    return Results.BadRequest("Invalid email format.");
                if (!validator.IsValidPassword(dto.Password))
                    return Results.BadRequest("Password must be at least 8 characters, include letters and numbers.");
                if (await validator.UserExistsAsync(dto.Username, dto.Email) == true)
                    return Results.Conflict("Username or Email already exists.");
                if (!Enum.TryParse<UserRole>(dto.Role, true, out UserRole role))
                    return Results.BadRequest("Invalid role. Allowed values: Client, Trainer, Admin.");

                using var transaction = await db.Database.BeginTransactionAsync(); // Start transaction

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    CreatedUtc = DateTime.UtcNow,
                    IsActive = true,
                    Role = role
                };
                await unitOfWork.Users.AddUserAsync(user); // Save user and set Id

                if (role == UserRole.Client)
                {
                    var client = new Client
                    {
                        UserId = user.Id,
                        User = user
                    };
                    await unitOfWork.Clients.AddClientAsync(client);
                }
                else if (role == UserRole.Trainer)
                {
                    var trainer = new Trainer
                    {
                        UserId = user.Id,
                        User = user
                    };
                    await unitOfWork.Trainers.AddTrainerAsync(trainer);
                }
                await transaction.CommitAsync(); // Commit transaction
                return Results.Created($"/users/{user.Id}", $"User {user.Username} with id {user.Id} registered successfully.");
            });
            #endregion

            #region Login
            app.MapPost("/login", async (
                HttpContext httpContext,
                IJwtService jwtService,
                LoginDto dto,
                IConfiguration config,
                IUnitOfWork unitOfWork
            ) =>
            {
                var user = await unitOfWork.Users.GetUserByUsernameAsync(dto.Username);
                if (user is null || !await unitOfWork.Users.ValidateUserCredentialsAsync(dto.Username, dto.Password))
                    return Results.Unauthorized();
                if (!await unitOfWork.Users.IsUserActiveAsync(dto.Username))
                    return Results.Forbid();

                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
                    return Results.Problem("JWT configuration missing.");

                var accessTokenMinutes = config.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 30;
                var refreshTokenDays = config.GetValue<int?>("Jwt:RefreshTokenDays") ?? 7;
                var ipAddress = ResolveClientIp(httpContext);

                var tokenPair = await jwtService.GenerateTokenPairAsync(
                    user,
                    jwtKey,
                    jwtIssuer,
                    TimeSpan.FromMinutes(accessTokenMinutes),
                    TimeSpan.FromDays(refreshTokenDays),
                    ipAddress);

                var response = new TokenPairDto(
                    tokenPair.AccessToken,
                    tokenPair.AccessTokenExpiresUtc,
                    tokenPair.RefreshToken,
                    tokenPair.RefreshTokenExpiresUtc);

                return Results.Ok(response);
            }).RequireRateLimiting("login");
            #endregion

            #region Refresh
            app.MapPost("/refresh", async (
                HttpContext httpContext,
                RefreshTokenRequestDto dto,
                IConfiguration config,
                IJwtService jwtService
            ) =>
            {
                if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                {
                    return Results.BadRequest("Refresh token is required.");
                }

                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
                    return Results.Problem("JWT configuration missing.");

                var accessTokenMinutes = config.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 30;
                var refreshTokenDays = config.GetValue<int?>("Jwt:RefreshTokenDays") ?? 7;
                var ipAddress = ResolveClientIp(httpContext);

                var result = await jwtService.RefreshTokenAsync(
                    dto.RefreshToken,
                    jwtKey,
                    jwtIssuer,
                    TimeSpan.FromMinutes(accessTokenMinutes),
                    TimeSpan.FromDays(refreshTokenDays),
                    ipAddress);

                if (!result.Success || result.Tokens is null)
                {
                    return Results.Json(
                        new { message = result.Error ?? "Unable to refresh token." },
                        statusCode: StatusCodes.Status401Unauthorized);
                }

                var tokens = result.Tokens;

                var response = new TokenPairDto(
                    tokens.AccessToken,
                    tokens.AccessTokenExpiresUtc,
                    tokens.RefreshToken,
                    tokens.RefreshTokenExpiresUtc);

                return Results.Ok(response);
            });
            #endregion

            static string? ResolveClientIp(HttpContext context)
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                {
                    var candidate = forwardedFor.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(candidate))
                    {
                        return candidate.Split(',').First().Trim();
                    }
                }

                return context.Connection.RemoteIpAddress?.ToString();
            }
        }
        
    }
}