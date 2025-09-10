
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.Dtos;
using Api.Services;

namespace Api.Endpoints
{
	public static class UsersEndpoints
	{
		public static void MapUserEndpoints(this WebApplication app)
		{
			#region User Registration and Login
			// Use default authorization (JWT)
			#region Register
			app.MapPost("/register", async (IValidationService validator, RegisterUserDto dto, AppDbContext db, IConfiguration config) =>
			{
				if (!validator.IsValidEmail(dto.Email))
					return Results.BadRequest("Invalid email format.");
				if (!validator.IsValidPassword(dto.Password))
					return Results.BadRequest("Password must be at least 8 characters, include letters and numbers.");
				if (await validator.UserExistsAsync(dto.Username, dto.Email) == true)
					return Results.Conflict("Username or Email already exists.");

				UserRole role;
				if (!Enum.TryParse<UserRole>(dto.Role, true, out role))
					role = UserRole.Client;


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
				db.Users.Add(user);
				if (role == UserRole.Client)
				{
					var client = new Client
					{
						UserId = user.Id,
						User = user
					};
					db.Clients.Add(client);
				}
				else if (role == UserRole.Trainer)
				{
					var trainer = new Trainer
					{
						UserId = user.Id,
						User = user
					};
					db.Trainers.Add(trainer);
				}
				await db.SaveChangesAsync();
				return Results.Created($"/users/{user.Id}", new UserDto(user.Id, user.Username, user.Email, user.IsActive, user.Role, user.ProfilePhotoUrl,user.TrainerProfile,user.ClientProfile));
			});
			#endregion
			#region Login
			app.MapPost("/login", async (IJwtService jwtService, IValidationService validator, LoginDto dto, AppDbContext db, IConfiguration config) =>
			{
				if (!validator.IsValidPassword(dto.Password))
					return Results.BadRequest("Invalid password format.");
				var user = await db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
				if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
					return Results.Unauthorized();

				// Create JWT token
				var jwtKey = config["Jwt:Key"];
				var jwtIssuer = config["Jwt:Issuer"];
				if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
					return Results.Problem("JWT configuration missing.");

				var tokenString = jwtService.GenerateToken(user, jwtKey, jwtIssuer);
				return Results.Ok(new { token = tokenString });
			});
			#endregion
			#endregion
			#region Admin Operations
			// Admin:Create User
			app.MapPost("/users", async (CreateUserDto dto, AppDbContext db, IValidationService validator) =>
			{
				if (!validator.IsValidEmail(dto.Email))
					return Results.BadRequest("Invalid email format.");
				if (await validator.UserExistsAsync(dto.Username, dto.Email))
					return Results.Conflict("Username or Email already exists.");

				var tempPassword = Guid.NewGuid().ToString().Substring(0, 8) + "1aA"; // Temporary password
				var passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
				var user = new User
				{
					Username = dto.Username,
					Email = dto.Email,
					PasswordHash = passwordHash,
					CreatedUtc = DateTime.UtcNow,
					IsActive = dto.IsActive,
					Role = dto.Role == default ? UserRole.Client : dto.Role,
					ProfilePhotoUrl = dto.ProfilePhotoUrl
				};
				db.Users.Add(user);
				if (user.Role == UserRole.Client)
				{
					var client = new Client
					{
						UserId = user.Id,
						User = user
					};
					db.Clients.Add(client);
				}
				else if (user.Role == UserRole.Trainer)
				{
					var trainer = new Trainer
					{
						UserId = user.Id,
						User = user
					};
					db.Trainers.Add(trainer);
				}
				await db.SaveChangesAsync();
				return Results.Created($"/users/{user.Id}", new { User = new UserDto(user.Id, user.Username, user.Email,user.IsActive, user.Role, user.ProfilePhotoUrl,user.TrainerProfile,user.ClientProfile), TemporaryPassword = tempPassword });
			}).RequireAuthorization("Admin");

			// Admin:Read (all) Users
			app.MapGet("/users", async (AppDbContext db) =>
			{
				var users = await db.Users
					.AsNoTracking()
					.Select(u => new UserDto(u.Id, u.Username, u.Email,u.IsActive, u.Role, u.ProfilePhotoUrl,u.TrainerProfile,u.ClientProfile))
					.ToListAsync();
				return Results.Ok(users);
			}).RequireAuthorization("Admin");

			// Admin:Read (one) User
			app.MapGet("/users/{id:int}", async (int id, AppDbContext db) =>
			{
				var u = await db.Users
					.AsNoTracking()
					.FirstOrDefaultAsync(x => x.Id == id);
				return u is null ? Results.NotFound() : Results.Ok(new UserDto(u.Id, u.Username, u.Email, u.IsActive, u.Role, u.ProfilePhotoUrl,u.TrainerProfile,u.ClientProfile));
			}).RequireAuthorization("Admin");

			// Admin:Update (one) User
			app.MapPut("/users/{id:int}", async (int id, CreateUserDto dto, AppDbContext db, IValidationService validator) =>
			{
				var u = await db.Users.FindAsync(id);
				if (u is null) return Results.NotFound();
				if (!validator.IsValidEmail(dto.Email))
					return Results.BadRequest("Invalid email format.");
				if (u.Username != dto.Username || u.Email != dto.Email)
				{
					if (await validator.UserExistsAsync(dto.Username, dto.Email))
						return Results.Conflict("Username or Email already exists.");
				}
				u.Username = dto.Username;
				u.Email = dto.Email;
				u.IsActive = dto.IsActive;
				u.Role = dto.Role == default ? UserRole.Client : dto.Role;
				await db.SaveChangesAsync();
				return Results.Ok(new UserDto(u.Id, u.Username, u.Email, u.IsActive, u.Role, u.ProfilePhotoUrl, u.TrainerProfile, u.ClientProfile));
			}).RequireAuthorization("Admin");

			// Admin:Delete (one) User
			app.MapDelete("/users/{id:int}", async (int id, AppDbContext db) =>
			{
				var u = await db.Users.FindAsync(id);
				if (u is null) return Results.NotFound();
				db.Users.Remove(u);
				await db.SaveChangesAsync();
				return Results.NoContent();
			}).RequireAuthorization("Admin");

			//Admin Switch User Role between Trainer and Client
			app.MapPost("/users/{id:int}/switch-role", async (int id, AppDbContext db) =>
			{
				var user = await db.Users.Include(u => u.ClientProfile).Include(u => u.TrainerProfile).FirstOrDefaultAsync(u => u.Id == id);
				if (user is null) return Results.NotFound("User not found.");
				if (user.Role == UserRole.Admin) return Results.BadRequest("Cannot change role of an Admin.");

				if (user.Role == UserRole.Client)
				{
					// Switch to Trainer
					if (user.ClientProfile != null)
					{
						db.Clients.Remove(user.ClientProfile);
						user.ClientProfile = null;
					}
					var trainer = new Trainer
					{
						UserId = user.Id,
						User = user
					};
					db.Trainers.Add(trainer);
					user.Role = UserRole.Trainer;
				}
				else if (user.Role == UserRole.Trainer)
				{
					// Switch to Client
					if (user.TrainerProfile != null)
					{
						db.Trainers.Remove(user.TrainerProfile);
						user.TrainerProfile = null;
					}
					var client = new Client
					{
						UserId = user.Id,
						User = user
					};
					db.Clients.Add(client);
					user.Role = UserRole.Client;
				}
				await db.SaveChangesAsync();
				return Results.Ok(new { message = $"User role switched to {user.Role}" });
			}).RequireAuthorization("Admin");

			//Admin:Assign Trainer or Client profile if a registration passed without a role
			app.MapPost("/users/{id:int}/assign-profile",async(int id,UserRole role,AppDbContext db,IValidationService validator)=>
			{
				var user = await db.Users.Include(u => u.ClientProfile).Include(u => u.TrainerProfile).FirstOrDefaultAsync(u => u.Id == id);
				if (user is null) return Results.NotFound("User not found.");
				if (user.Role != role) return Results.BadRequest("User role does not match the profile to be assigned.");
				if (!await validator.CanAssignProfile(id)) return Results.Conflict("User already has a profile assigned.");
				if (role == UserRole.Client)
				{
					var client = new Client
					{
						UserId = user.Id,
						User = user
					};
					db.Clients.Add(client);
					await db.SaveChangesAsync();
					return Results.Ok(new { message = "Client profile assigned successfully." });
				}
				else if (role == UserRole.Trainer)
				{
					var trainer = new Trainer
					{
						UserId = user.Id,
						User = user
					};
					db.Trainers.Add(trainer);
					await db.SaveChangesAsync();
					return Results.Ok(new { message = "Trainer profile assigned successfully." });
				}
				return Results.BadRequest("Invalid role.");
			}).RequireAuthorization("Admin");
			#endregion

		}
	}
}
