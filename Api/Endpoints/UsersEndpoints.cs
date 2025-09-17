using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.Dtos;
using Api.Services;
using Api.Repositories;


namespace Api.Endpoints
{
	public static class UsersEndpoints
	{
		public static void MapUserEndpoints(this WebApplication app)
		{
			#region User Registration and Login
			// Use default authorization (JWT)
			#region Register
			app.MapPost("/register", async (
				IValidationService validator,
				RegisterUserDto dto,
				AppDbContext db,
				IConfiguration config,
				IUserRepository userRepository,
				IClientRepository clientRepository,
				ITrainerRepository trainerRepository
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
				await userRepository.AddUserAsync(user); // Save user and set Id

				if (role == UserRole.Client)
				{
					var client = new Client
					{
						UserId = user.Id,
						User = user
					};
					await clientRepository.AddClientAsync(client);
				}
				else if (role == UserRole.Trainer)
				{
					var trainer = new Trainer
					{
						UserId = user.Id,
						User = user
					};
					await trainerRepository.AddTrainerAsync(trainer);
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
				IUserRepository userRepository
			) =>
			{
				var user = await userRepository.GetUserByUsernameAsync(dto.Username);
				if (user is null || !await userRepository.ValidateUserCredentialsAsync(dto.Username, dto.Password))
					return Results.Unauthorized();
				if (!await userRepository.IsUserActiveAsync(dto.Username))
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

			#region Admin:Read All Users

			app.MapGet("/users", async (
				IUserRepository userRepository,
				IPaginationService paginationService,
				int? page,
				int? pageSize,
				string? sortBy,
				string? sortOrder
			) =>
			{
				var (users, total) = await userRepository.GetUsersPagedAsync(paginationService, page, pageSize, sortBy, sortOrder);
				return Results.Ok(new { users, total });
			}).RequireAuthorization("Admin");
			#endregion

			#region Admin:Switch User Role
			app.MapPost("/users/{id:int}/switch-role", async (
							int id,
							IUserRepository userRepository,
							IClientRepository clientRepository,
							ITrainerRepository trainerRepository
						) =>
						{
							var (success, message, newRole) = await userRepository.SwitchUserRoleAsync(id, clientRepository, trainerRepository);
							if (!success)
							{
								if (message == "User not found.")
									return Results.NotFound(message);
								if (message == "Cannot change role of an Admin.")
									return Results.BadRequest(message);
								return Results.BadRequest(message);
							}
							return Results.Ok(new { message = $"User role switched to {newRole}" });
						}).RequireAuthorization("Admin");
			#endregion

			#region Admin:Assign User Profile if nonexistent
			app.MapPost("/users/{id:int}/assign-profile", async (
				int id,
				UserRole role,
				IUserRepository userRepository,
				IClientRepository clientRepository,
				ITrainerRepository trainerRepository,
				IValidationService validator
			) =>
			{
				var (success, message) = await userRepository.AssignProfileAsync(id, role, clientRepository, trainerRepository, validator);
				if (!success)
				{
					if (message == "User not found.")
						return Results.NotFound(message);
					if (message == "User already has a profile assigned.")
						return Results.Conflict(message);
					return Results.BadRequest(message);
				}
				return Results.Ok(new { message });
			}).RequireAuthorization("Admin");
			#endregion

			#region Admin: Read One User By Id
			app.MapGet("/users/{id:int}", async (
				int id,
				IUserRepository userRepository
			) =>
			{
				var userDto = await userRepository.GetUserWithProfilesByIdAsync(id);
				if (userDto == null) return Results.NotFound("User not found.");
				return Results.Ok(userDto);
			}).RequireAuthorization("Admin");
			#endregion

			#region Admin:Create new User
			app.MapPost("/users", async (
				IValidationService validator,
				RegisterUserDto dto,
				IUserRepository userRepository,
				IClientRepository clientRepository,
				ITrainerRepository trainerRepository
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
				await userRepository.AddUserAsync(user); // Save user and set Id

				if (role == UserRole.Client)
				{
					var client = new Client
					{
						UserId = user.Id,
						User = user
					};
					await clientRepository.AddClientAsync(client);
				}
				else if (role == UserRole.Trainer)
				{
					var trainer = new Trainer
					{
						UserId = user.Id,
						User = user
					};
					await trainerRepository.AddTrainerAsync(trainer);
				}
				return Results.Created($"/users/{user.Id}", $"User {user.Username} with id {user.Id} created successfully.");
			}).RequireAuthorization("Admin");
			#endregion

			#region Admin:Delete User
			app.MapDelete("/users/{id:int}", async (
				int id,
				IUserRepository userRepository
			) =>
			{
				var (success, message) = await userRepository.DeleteUserAsync(id);
				if (!success)
				{
					if (message == "User not found.")
						return Results.NotFound(message);
					if (message == "Cannot delete an Admin user.")
						return Results.BadRequest(message);
					return Results.BadRequest(message);
				}
				return Results.Ok(new { message = "User deleted successfully." });
			}).RequireAuthorization("Admin");
			#endregion

			#endregion

			#region User Profile Operations

			#region Get Current Logged User Profile
			app.MapGet("/users/me", async (
				HttpContext context,
				IUserRepository userRepository
			) =>
			{
				var userIdClaim = context.User.FindFirst("userid")?.Value;
				if (userIdClaim == null) return Results.Unauthorized();

				if (!int.TryParse(userIdClaim, out int userId))
					return Results.Unauthorized();

				var userDto = await userRepository.GetUserWithProfilesByIdAsync(userId);
				if (userDto == null) return Results.NotFound("User not found.");
				return Results.Ok(userDto);
			}).RequireAuthorization();
			#endregion

			#region Handle Photo Upload
			app.MapPost("/users/{id:int}/upload-photo", async (
				int id,
				HttpContext context,
				IUserRepository userRepository,
				IValidationService validator,
				IWebHostEnvironment env
			) =>
			{
				var userIdClaim = context.User.FindFirst("userid")?.Value;
				if (userIdClaim == null || int.Parse(userIdClaim) != id)
					return Results.Forbid();

				if (!context.Request.HasFormContentType)
					return Results.BadRequest("Invalid form data.");

				var form = await context.Request.ReadFormAsync();
				var file = form.Files.GetFile("photo");
				if (file is null || file.Length == 0)
					return Results.BadRequest("No file uploaded.");

				if (!validator.IsValidImage(file))
					return Results.BadRequest("Invalid image file. Only JPEG and PNG formats under 2MB are allowed.");

				var (success, message, photoUrl) = await userRepository.UploadUserProfilePhotoAsync(id, file, env);
				if (!success)
				{
					if (message == "User not found.")
						return Results.NotFound(message);
					return Results.BadRequest(message);
				}
				return Results.Ok(new { message = "Profile photo uploaded successfully.", photoUrl });
			}).RequireAuthorization();
			#endregion

			#endregion
		}
	}
}
