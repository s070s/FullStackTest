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
                // Basic validation: email format and password constraints
                if (!validator.IsValidEmail(dto.Email))
                    return Results.BadRequest("Invalid email format.");
                if (!validator.IsValidPassword(dto.Password))
                    return Results.BadRequest("Password must be at least 8 characters, include letters and numbers.");
                if (await validator.UserExistsAsync(dto.Username, dto.Email) == true)
                    return Results.Conflict("Username or Email already exists.");
                if (!Enum.TryParse<UserRole>(dto.Role, true, out UserRole role))
                    return Results.BadRequest("Invalid role. Allowed values: Client, Trainer, Admin.");

                // Use a database transaction to ensure user + role-specific record are created atomically.
                // If any operation fails, the transaction should be rolled back automatically when disposed
                // (or explicitly on exception).
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

                // Use the UnitOfWork to persist the user. This abstracts repository logic and helps
                // with testing and separation of concerns.
                await unitOfWork.Users.AddUserAsync(user); // Save user and set Id

                // Create related domain record depending on role.
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

                // Commit after both user and role-specific records are saved.
                await transaction.CommitAsync(); // Commit transaction

                // Return Created with location and a simple success message.
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
                // Fetch user and validate credentials via UnitOfWork / repository.
                var user = await unitOfWork.Users.GetUserByUsernameAsync(dto.Username);
                if (user is null || !await unitOfWork.Users.ValidateUserCredentialsAsync(dto.Username, dto.Password))
                    return Results.Unauthorized();
                if (!await unitOfWork.Users.IsUserActiveAsync(dto.Username))
                    return Results.Forbid();

                // Ensure JWT configuration is present; fail early if misconfigured.
                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
                    return Results.Problem("JWT configuration missing.");

                // Token lifetime configuration with sensible defaults.
                var accessTokenMinutes = config.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 30;
                var refreshTokenDays = config.GetValue<int?>("Jwt:RefreshTokenDays") ?? 7;
                var ipAddress = ResolveClientIp(httpContext);

                // Generate access + refresh token pair. IJwtService handles signing and refresh storage.
                var tokenPair = await jwtService.GenerateTokenPairAsync(
                    user,
                    jwtKey,
                    jwtIssuer,
                    TimeSpan.FromMinutes(accessTokenMinutes),
                    TimeSpan.FromDays(refreshTokenDays),
                    ipAddress);


                // Set HttpOnly cookie for refresh token. Important security notes:
                // - HttpOnly prevents JS access to the cookie.
                // - Secure should be true in production (HTTPS enforced).
                // - SameSite=None allows cross-site usage (required if frontend and API are on different sites),
                //   but it is less strict; ensure you understand the implications and set cookie Domain if needed.
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,            // ensure HTTPS in production
                    SameSite = SameSiteMode.None, // allow cross-site (e.g., frontend hosted separately)
                    Expires = tokenPair.RefreshTokenExpiresUtc,
                    Path = "/"
                };
                httpContext.Response.Cookies.Append("refreshToken", tokenPair.RefreshToken, cookieOptions);


                return Results.Ok(new { accessToken = tokenPair.AccessToken, accessTokenExpiresUtc = tokenPair.AccessTokenExpiresUtc });
            }).RequireRateLimiting("login");
            #endregion

            #region Refresh
            // Refresh reads the refresh token from the HttpOnly cookie (no token in request body).
            // It rotates the refresh token and issues a new access token. On failure, clears cookie.
            app.MapPost("/refresh", async (
                HttpContext httpContext,
                IConfiguration config,
                IJwtService jwtService
            ) =>
             {
                var refreshToken = httpContext.Request.Cookies["refreshToken"];
                if (string.IsNullOrWhiteSpace(refreshToken))
                    return Results.Json(new { message = "Refresh token missing." }, statusCode: StatusCodes.Status401Unauthorized);

                // Ensure JWT settings exist.
                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
                    return Results.Problem("JWT configuration missing.");
                var accessTokenMinutes = config.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 30;
                var refreshTokenDays = config.GetValue<int?>("Jwt:RefreshTokenDays") ?? 7;

                // Attempt to refresh; IJwtService should validate token, check revocation, and rotate token.
                var result = await jwtService.RefreshTokenAsync(
                    refreshToken,
                    jwtKey,
                    jwtIssuer,
                    TimeSpan.FromMinutes(accessTokenMinutes),
                    TimeSpan.FromDays(refreshTokenDays),
                    ResolveClientIp(httpContext));

                // If refresh failed, clear cookie to avoid reuse on client and return 401.
                if (!result.Success || result.Tokens is null)
                {
                    httpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.UtcNow.AddDays(-1), Path = "/" });
                    return Results.Json(new { message = result.Error ?? "Unable to refresh token." }, statusCode: StatusCodes.Status401Unauthorized);
                }

                var tokens = result.Tokens;

                // Set rotated refresh token cookie (token rotation).
                httpContext.Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = tokens.RefreshTokenExpiresUtc,
                    Path = "/"
                });

                // Return new access token to the client.
                return Results.Ok(new { accessToken = tokens.AccessToken, accessTokenExpiresUtc = tokens.AccessTokenExpiresUtc });
            });
            #endregion

            #region Logout
            // Logout clears the refresh token cookie on the client.
            // Consider also revoking the refresh token server-side if stored.
            app.MapPost("/logout", (HttpContext httpContext) =>
            {
                httpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Path = "/"
                });
                return Results.Ok(new { message = "Logged out" });
            });
            #endregion

            // ResolveClientIp attempts to determine the client's IP in proxy scenarios using X-Forwarded-For header.
            // This is useful for binding refresh tokens to client IP to make token theft harder.
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