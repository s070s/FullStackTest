using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.Dtos;
using Api.Services;
using Api.Repositories.UnitOfWork;

namespace Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            // Map authentication routes (JWT-based)
            #region Register
            // Register a user and create the matching role profile in one DB transaction
            app.MapPost("/register", async (
                IValidationService validator,
                RegisterUserDto dto,
                AppDbContext db,
                IConfiguration config,
                IUnitOfWork unitOfWork
            ) =>
            {
                // Validate email format
                if (!validator.IsValidEmail(dto.Email))
                    return Results.BadRequest("Invalid email format.");

                // Validate password strength
                if (!validator.IsValidPassword(dto.Password))
                    return Results.BadRequest("Password must be at least 8 characters, include letters and numbers.");

                // Check if username or email already exists
                if (await validator.UserExistsAsync(dto.Username, dto.Email) == true)
                    return Results.Conflict("Username or Email already exists.");

                // Validate role
                if (!Enum.TryParse<UserRole>(dto.Role, true, out UserRole role))
                    return Results.BadRequest("Invalid role. Allowed values: Client, Trainer, Admin.");

                // Begin a database transaction to ensure atomicity
                using var transaction = await db.Database.BeginTransactionAsync();
                try
                {
                    // Hash the user's password
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                    // Create the user entity
                    var user = new User
                    {
                        Username = dto.Username,
                        Email = dto.Email,
                        PasswordHash = passwordHash,
                        CreatedUtc = DateTime.UtcNow,
                        IsActive = true,
                        Role = role
                    };

                    // Add user to the database (ID will be generated here)
                    await unitOfWork.Users.AddUserAsync(user);

                    // Create and link the appropriate profile based on role
                    if (role == UserRole.Client)
                    {
                        var client = new Client
                        {
                            UserId = user.Id,
                            User = user
                        };
                        await unitOfWork.Clients.AddClientAsync(client);
                        user.ClientProfile = client;
                        await unitOfWork.Users.UpdateUserAsync(user); // Link profile to user
                    }
                    else if (role == UserRole.Trainer)
                    {
                        var trainer = new Trainer
                        {
                            UserId = user.Id,
                            User = user
                        };
                        await unitOfWork.Trainers.AddTrainerAsync(trainer);
                        user.TrainerProfile = trainer;
                        await unitOfWork.Users.UpdateUserAsync(user); // Link profile to user
                    }
                    // Note: No profile is created for Admins

                    // Commit the transaction if all operations succeed
                    await transaction.CommitAsync();

                    // Return a success response with the new user's info
                    return Results.Created($"/users/{user.Id}", $"User {user.Username} with id {user.Id} registered successfully.");
                }
                catch (Exception ex)
                {
                    // Roll back the transaction on any error to maintain data integrity
                    await transaction.RollbackAsync();
                    // Log the exception for diagnostics
                    Console.Error.WriteLine($"Registration failed: {ex.Message}");
                    // Return a generic error response
                    return Results.Problem("Registration failed. Please try again.");
                }
            });
            #endregion

            #region Login
            // Authenticate user and issue access/refresh tokens
            app.MapPost("/login", async (
                HttpContext httpContext,
                IJwtService jwtService,
                LoginDto dto,
                IConfiguration config,
                IUnitOfWork unitOfWork
            ) =>
            {
                // Lookup and verify credentials
                var user = await unitOfWork.Users.GetUserByUsernameAsync(dto.Username);
                if (user is null || !await unitOfWork.Users.ValidateUserCredentialsAsync(dto.Username, dto.Password))
                    return Results.Unauthorized();
                if (!await unitOfWork.Users.IsUserActiveAsync(dto.Username))
                    return Results.Forbid();

                // Read JWT settings
                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
                    return Results.Problem("JWT configuration missing.");

                // Token lifetimes
                var accessTokenMinutes = config.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 30;
                var refreshTokenDays = config.GetValue<int?>("Jwt:RefreshTokenDays") ?? 7;
                var ipAddress = ResolveClientIp(httpContext);

                // Create token pair
                var tokenPair = await jwtService.GenerateTokenPairAsync(
                    user,
                    jwtKey,
                    jwtIssuer,
                    TimeSpan.FromMinutes(accessTokenMinutes),
                    TimeSpan.FromDays(refreshTokenDays),
                    ipAddress);

                // Determine if request is over HTTPS
                var isHttps = httpContext.Request.IsHttps || string.Equals(httpContext.Request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase);
                // Write refresh token cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = isHttps,
                    SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                    Expires = tokenPair.RefreshTokenExpiresUtc,
                    Path = "/"
                };
                httpContext.Response.Cookies.Append("refreshToken", tokenPair.RefreshToken, cookieOptions);

                // Return access token
                return Results.Ok(new { accessToken = tokenPair.AccessToken, accessTokenExpiresUtc = tokenPair.AccessTokenExpiresUtc });
            }).RequireRateLimiting("login");
            #endregion

            #region Refresh
            // Rotate the refresh token from cookie and return a fresh access token
            app.MapPost("/refresh", async (
                HttpContext httpContext,
                IConfiguration config,
                IJwtService jwtService
            ) =>
             {
                 // Read refresh token from cookie
                 var refreshToken = httpContext.Request.Cookies["refreshToken"];
                 if (string.IsNullOrWhiteSpace(refreshToken))
                     return Results.Json(new { message = "Refresh token missing." }, statusCode: StatusCodes.Status401Unauthorized);

                 // Read JWT settings
                 var jwtKey = config["Jwt:Key"];
                 var jwtIssuer = config["Jwt:Issuer"];
                 if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
                     return Results.Problem("JWT configuration missing.");
                 var accessTokenMinutes = config.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 30;
                 var refreshTokenDays = config.GetValue<int?>("Jwt:RefreshTokenDays") ?? 7;

                 // Determine if request is over HTTPS
                 var isHttps = httpContext.Request.IsHttps || string.Equals(httpContext.Request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase);

                 // Try to validate and rotate refresh token
                 var result = await jwtService.RefreshTokenAsync(
                     refreshToken,
                     jwtKey,
                     jwtIssuer,
                     TimeSpan.FromMinutes(accessTokenMinutes),
                     TimeSpan.FromDays(refreshTokenDays),
                     ResolveClientIp(httpContext));

                 // On failure, clear cookie and return 401
                 if (!result.Success || result.Tokens is null)
                 {
                     httpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
                     {
                         HttpOnly = true,
                         Secure = isHttps,
                         SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                         Expires = DateTime.UtcNow.AddDays(-1),
                         Path = "/"
                     });
                     return Results.Json(new { message = result.Error ?? "Unable to refresh token." }, statusCode: StatusCodes.Status401Unauthorized);
                 }

                 var tokens = result.Tokens;

                 // Write rotated refresh token cookie
                 httpContext.Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
                 {
                     HttpOnly = true,
                     SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                     Secure = isHttps,
                     Expires = tokens.RefreshTokenExpiresUtc,
                     Path = "/"
                 });

                 // Return new access token
                 return Results.Ok(new { accessToken = tokens.AccessToken, accessTokenExpiresUtc = tokens.AccessTokenExpiresUtc });
             });
            #endregion

            #region Logout
            // Remove the refresh token cookie from the client and revoke it server-side
            app.MapPost("/logout", async (HttpContext httpContext, IJwtService jwtService) =>
            {
                // Determine if request is over HTTPS
                var isHttps = httpContext.Request.IsHttps || string.Equals(httpContext.Request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase); var refreshToken = httpContext.Request.Cookies["refreshToken"];
                
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    // Try to extract userId from JWT or session if available, otherwise skip userId check
                    // If you have userId in claims, extract it here:
                    int? userId = null;
                    if (httpContext.User.Identity?.IsAuthenticated == true)
                    {
                        var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "userid");
                        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedId))
                            userId = parsedId;
                    }
                    // If userId is not available, you may want to look up by token only (less secure)
                    if (userId.HasValue)
                    {
                        await jwtService.RevokeRefreshTokenAsync(userId.Value, refreshToken, httpContext.Connection.RemoteIpAddress?.ToString());
                    }
                    // Optionally: If userId is not available, you could try to revoke by token only
                }
                // Expire cookie immediately
                httpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = isHttps,
                    SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Path = "/"
                });
                return Results.Ok(new { message = "Logged out" });
            });
            #endregion

            // Best-effort client IP (prefers X-Forwarded-For; falls back to remote address)
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