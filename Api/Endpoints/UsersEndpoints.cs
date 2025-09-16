using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.Dtos;
using Api.Services;
using System.Security.Claims;

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
				db.Users.Add(user);
				await db.SaveChangesAsync(); // Important to set the userId before the Role

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
				await db.SaveChangesAsync();//Second Save Operation to update the Roles
				await transaction.CommitAsync(); // Commit transaction
				return Results.Created($"/users/{user.Id}", $"User {user.Username} with id {user.Id} registered successfully.");
			});
			#endregion
			#region Login
			app.MapPost("/login", async (IValidationService validator, IJwtService jwtService, LoginDto dto, AppDbContext db, IConfiguration config) =>
			{
				var user = await db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
				//no validation for password requirements for redundancy and security reasons(error messages),there is only
				//validation to verify if the password matches the hash stored in the database
				if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
					return Results.Unauthorized();
				if (!await validator.IsUserActiveAsync(dto.Username))
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
			#endregion
			#region Admin Operations
			// Admin:Read (all) Users
			app.MapGet("/users", async (
				AppDbContext db,
				IPaginationService paginationService,
				int? page,
				int? pageSize,
				string? sortBy,
				string? sortOrder
			) =>
			{
				var query = db.Users.AsNoTracking();

				var total = await query.CountAsync();

				query = paginationService.ApplySorting(query, sortBy, sortOrder);
				query = paginationService.ApplyPagination(query, page, pageSize);

				var users = await query
					.Select(u => new UserDto(
						u.Id,
						u.CreatedUtc,
						u.Username,
						u.Email,
						u.IsActive,
						u.Role,
						u.ProfilePhotoUrl,
						u.TrainerProfile != null ? u.TrainerProfile.ToTrainerDto() : null,
						u.ClientProfile != null ? u.ClientProfile.ToClientDto() : null
					))
					.ToListAsync();

				return Results.Ok(new { users, total });
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
			app.MapPost("/users/{id:int}/assign-profile", async (int id, UserRole role, AppDbContext db, IValidationService validator) =>
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



			#region User Profile Operations

			// Get Current Logged User Profile
			app.MapGet("/users/me", async (HttpContext context, AppDbContext db) =>
			{
				var userIdClaim = context.User.FindFirst("userid")?.Value;
				if (userIdClaim == null) return Results.Unauthorized();

				if (!int.TryParse(userIdClaim, out int userId))
					return Results.Unauthorized();

				var user = await db.Users
					.AsNoTracking()
					.Include(u => u.TrainerProfile)
					.Include(u => u.ClientProfile)
					.SingleOrDefaultAsync(u => u.Id == userId);

				if (user is null) return Results.NotFound("User not found.");

				// If the profile is empty (only UserId and User), treat as null
				TrainerDto? trainerProfile = null;
				if (user.TrainerProfile != null && !user.TrainerProfile.GetType().GetProperties().All(p =>
					p.Name == "UserId" || p.Name == "User" || p.GetValue(user.TrainerProfile) == null))
				{
					trainerProfile = user.TrainerProfile.ToTrainerDto();
				}

				ClientDto? clientProfile = null;
				if (user.ClientProfile != null && !user.ClientProfile.GetType().GetProperties().All(p =>
					p.Name == "UserId" || p.Name == "User" || p.GetValue(user.ClientProfile) == null))
				{
					clientProfile = user.ClientProfile.ToClientDto();
				}

				var userDto = new UserDto(
					user.Id,
					user.CreatedUtc,
					user.Username,
					user.Email,
					user.IsActive,
					user.Role,
					user.ProfilePhotoUrl,
					trainerProfile,
					clientProfile
				);
				return Results.Ok(userDto);
			}).RequireAuthorization();



			// Handle Photo Upload
			app.MapPost("/users/{id:int}/upload-photo", async (int id, HttpContext context, AppDbContext db, IValidationService validator, IWebHostEnvironment env) =>
			{

				var userIdClaim = context.User.FindFirst("userid")?.Value;
				if (userIdClaim == null || int.Parse(userIdClaim) != id)
					return Results.Forbid();

				var user = await db.Users.FindAsync(id);
				if (user is null) return Results.NotFound("User not found.");

				if (!context.Request.HasFormContentType)
					return Results.BadRequest("Invalid form data.");

				var form = await context.Request.ReadFormAsync();
				var file = form.Files.GetFile("photo");
				if (file is null || file.Length == 0)
					return Results.BadRequest("No file uploaded.");

				if (!validator.IsValidImage(file))
					return Results.BadRequest("Invalid image file. Only JPEG and PNG formats under 2MB are allowed.");

				// Delete old photo if it exists
				if (!string.IsNullOrEmpty(user.ProfilePhotoUrl))
				{
					var oldFilePath = Path.Combine(env.WebRootPath ?? "wwwroot", user.ProfilePhotoUrl.TrimStart('/', '\\'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				var uploadsDir = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads");
				if (!Directory.Exists(uploadsDir))
					Directory.CreateDirectory(uploadsDir);

				var fileExtension = Path.GetExtension(file.FileName);
				var newFileName = $"user_{id}_{Guid.NewGuid()}{fileExtension}";
				var filePath = Path.Combine(uploadsDir, newFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				// Update user's profile photo URL
				user.ProfilePhotoUrl = $"/uploads/{newFileName}";
				await db.SaveChangesAsync();

				return Results.Ok(new { message = "Profile photo uploaded successfully.", photoUrl = user.ProfilePhotoUrl });
			}).RequireAuthorization();
			#endregion
		}
	}
}
