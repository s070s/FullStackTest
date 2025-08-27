
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.Dtos;

namespace Api.Endpoints
{
	public static class UsersEndpoints
	{
		public static void MapUserEndpoints(this WebApplication app)
		{
			// Create
			app.MapPost("/users", async (CreateUserDto dto, AppDbContext db) =>
			{
				var exists = await db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
				if (exists) return Results.Conflict("Username or Email already exists.");

				var user = new User { Username = dto.Username, Email = dto.Email, CreatedUtc = DateTime.UtcNow, IsActive = true };
				db.Users.Add(user);
				await db.SaveChangesAsync();
				return Results.Created($"/users/{user.Id}", new UserDto(user.Id, user.Username, user.Email, user.CreatedUtc, user.IsActive));
			});

			// Read (all)
			app.MapGet("/users", async (AppDbContext db) =>
				Results.Ok(await db.Users.OrderBy(u => u.Id)
					.Select(u => new UserDto(u.Id, u.Username, u.Email, u.CreatedUtc, u.IsActive))
					.ToListAsync()));

			// Read (one)
			app.MapGet("/users/{id:int}", async (int id, AppDbContext db) =>
			{
				var u = await db.Users.FindAsync(id);
				return u is null ? Results.NotFound() : Results.Ok(new UserDto(u.Id, u.Username, u.Email, u.CreatedUtc, u.IsActive));
			});

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

				await db.SaveChangesAsync();
				return Results.NoContent();
			});

			// Delete
			app.MapDelete("/users/{id:int}", async (int id, AppDbContext db) =>
			{
				var u = await db.Users.FindAsync(id);
				if (u is null) return Results.NotFound();
				db.Users.Remove(u);
				await db.SaveChangesAsync();
				return Results.NoContent();
			});
		}
	}
}
