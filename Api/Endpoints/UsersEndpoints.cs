
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Endpoints
{
	public static class UsersEndpoints
	{
		public static void MapUserEndpoints(this WebApplication app)
		{
			#region Registration
			// Use default authorization (JWT)
			// Register
			app.MapPost("/register", async (RegisterUserDto dto, AppDbContext db, IConfiguration config) =>
			{
				var exists = await db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
				if (exists) return Results.Conflict("Username or Email already exists.");

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
				await db.SaveChangesAsync();
				return Results.Created($"/users/{user.Id}", new UserDto(user.Id, user.Username, user.Email, user.CreatedUtc, user.IsActive, user.Role));
			});

			// Login
			app.MapPost("/login", async (LoginDto dto, AppDbContext db, IConfiguration config) =>
			{
				var user = await db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
				if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
					return Results.Unauthorized();

				// Create JWT token
				var jwtKey = config["Jwt:Key"];
				var jwtIssuer = config["Jwt:Issuer"];
				if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
					return Results.Problem("JWT configuration missing.");
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
				var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
				return Results.Ok(new { token = tokenString });
			});
			#endregion

			#region Admin Operations
			// Create (admin only)
			app.MapPost("/users", async (CreateUserDto dto, AppDbContext db) =>
			{
				var exists = await db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
				if (exists) return Results.Conflict("Username or Email already exists.");

				

				var user = new User
				{
					Username = dto.Username,
					Email = dto.Email,
					CreatedUtc = DateTime.UtcNow,
					IsActive = true,
					Role = dto.Role == default ? UserRole.Client : dto.Role
				};
				db.Users.Add(user);
				await db.SaveChangesAsync();
				return Results.Created($"/users/{user.Id}", new UserDto(user.Id, user.Username, user.Email, user.CreatedUtc, user.IsActive, user.Role));
			}).RequireAuthorization("Admin");

			// Read (all)
			app.MapGet("/users", async (AppDbContext db) =>
				Results.Ok(await db.Users.OrderBy(u => u.Id)
					.Select(u => new UserDto(u.Id, u.Username, u.Email, u.CreatedUtc, u.IsActive, u.Role))
					.ToListAsync())).RequireAuthorization("Admin");

			// Read (one)
			app.MapGet("/users/{id:int}", async (int id, AppDbContext db) =>
			{
				var u = await db.Users.FindAsync(id);
				return u is null ? Results.NotFound() : Results.Ok(new UserDto(u.Id, u.Username, u.Email, u.CreatedUtc, u.IsActive, u.Role));
			}).RequireAuthorization("Admin");

			// Update
			app.MapPut("/users/{id:int}", async (int id, UpdateUserDto dto, AppDbContext db) =>
			{
				var u = await db.Users.FindAsync(id);
				if (u is null) return Results.NotFound();

				if (!string.IsNullOrWhiteSpace(dto.Email) && !dto.Email.Equals(u.Email, StringComparison.OrdinalIgnoreCase))
				{
					var emailInUse = await db.Users.AnyAsync(x => x.Email == dto.Email && x.Id != id);
					if (emailInUse) return Results.Conflict("Email already in use.");
					u.Email = dto.Email!;
				}

				if (dto.IsActive.HasValue) u.IsActive = dto.IsActive.Value;
				u.Role = dto.Role == default ? UserRole.Client : dto.Role;
				await db.SaveChangesAsync();
				return Results.Ok(new UserDto(u.Id, u.Username, u.Email, u.CreatedUtc, u.IsActive, u.Role)); // Return updated user
			}).RequireAuthorization("Admin");

			// Delete
			app.MapDelete("/users/{id:int}", async (int id, AppDbContext db) =>
			{
				var u = await db.Users.FindAsync(id);
				if (u is null) return Results.NotFound();
				db.Users.Remove(u);
				await db.SaveChangesAsync();
				return Results.Ok(new { message = $"Successfully deleted user{u.Username} with id {id}"});
			}).RequireAuthorization("Admin");
			#endregion

		}
	}
}
