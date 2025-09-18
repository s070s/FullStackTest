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
                IValidationService validator,
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

                // Create JWT token
                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
                    return Results.Problem("JWT configuration missing.");

                var tokenString = jwtService.GenerateToken(user, jwtKey, jwtIssuer);
                return Results.Ok(new { token = tokenString });
            }).RequireRateLimiting("login");
            #endregion

        }
        
    }
}