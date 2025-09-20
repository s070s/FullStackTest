using Api.Models;
using Api.Models.Enums;
using Api.Dtos;
using Api.Services;
using Api.Repositories.UnitOfWork;


namespace Api.Endpoints
{
    public static class AdminUserEndpoints
    {
        public static void MapAdminUserEndpoints(this WebApplication app)
        {

            #region Admin:Read All Users
            app.MapGet("/users", async (
                IUnitOfWork unitOfWork,
                IPaginationService paginationService,
                int? page,
                int? pageSize,
                string? sortBy,
                string? sortOrder
            ) =>
            {
                var (users, total) = await unitOfWork.Users.GetUsersPagedAsync(paginationService, page, pageSize, sortBy, sortOrder);
                return Results.Ok(new { users, total });
            }).RequireAuthorization("Admin");
            #endregion

            #region Admin:Read All User Statistics
            app.MapGet("/users/statistics", async (
                IUnitOfWork unitOfWork
            ) =>
            {
                var stats = await unitOfWork.Users.GetUserStatisticsAsync();
                return Results.Ok(stats);
            }).RequireAuthorization("Admin");
            #endregion

            #region Admin:Switch User Role
            app.MapPost("/users/{id:int}/switch-role", async (
                            int id,
                            IUnitOfWork unitOfWork
                        ) =>
                        {
                            var (success, message, newRole) = await unitOfWork.Users.SwitchUserRoleAsync(id, unitOfWork.Clients, unitOfWork.Trainers);
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
                IUnitOfWork unitOfWork,
                IValidationService validator
            ) =>
            {
                var (success, message) = await unitOfWork.Users.AssignProfileAsync(id, role, unitOfWork.Clients, unitOfWork.Trainers, validator);
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
                IUnitOfWork unitOfWork
            ) =>
            {
                var userDto = await unitOfWork.Users.GetUserWithProfilesByIdAsync(id);
                if (userDto == null) return Results.NotFound("User not found.");
                return Results.Ok(userDto);
            }).RequireAuthorization("Admin");
            #endregion

            #region Admin:Create new User
            app.MapPost("/users", async (
                IValidationService validator,
                RegisterUserDto dto,
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
                return Results.Created($"/users/{user.Id}", $"User {user.Username} with id {user.Id} created successfully.");
            }).RequireAuthorization("Admin");
            #endregion

            #region Admin:Update User
            app.MapPut("/users/{id:int}", async (
                int id,
                UpdateUserDto dto,
                IValidationService validator,
                IUnitOfWork unitOfWork
            ) =>
            {
                var getUser = await unitOfWork.Users.GetUserByIdAsync(id);
                if (getUser == null)
                    return Results.NotFound("User not found.");
                if (!string.IsNullOrEmpty(dto.Email) && !validator.IsValidEmail(dto.Email))
                    return Results.BadRequest("Invalid email format.");
                if (!string.IsNullOrEmpty(dto.Password) && !validator.IsValidPassword(dto.Password))
                    return Results.BadRequest("Password must be at least 8 characters, include letters and numbers.");
                if (dto.Username != null)
                {
                    if (await unitOfWork.Users.UsernameExistsAsync(dto.Username, id))
                        return Results.Conflict("Username already exists.");
                }
                if (dto.Email != null)
                {
                    if (await unitOfWork.Users.EmailExistsAsync(dto.Email))
                        return Results.Conflict("Email already exists.");
                }
                if (dto.Role != null)
                {
                    var newRole = dto.Role.Value;
                    if (getUser.Role == UserRole.Admin && newRole != UserRole.Admin)
                        return Results.BadRequest("Cannot change role of an Admin.");
                }
                var updatedUser = new User
                {
                    Id = id,
                    Username = dto.Username ?? getUser.Username,
                    Email = dto.Email ?? getUser.Email,
                    IsActive = dto.IsActive ?? false,
                    Role = dto.Role ?? getUser.Role,
                    ProfilePhotoUrl = getUser.ProfilePhotoUrl,
                };
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                    updatedUser.PasswordHash = passwordHash;
                }
                if (dto.Username == null || string.IsNullOrEmpty(dto.Username))
                    updatedUser.Username = getUser.Username;
                if (dto.Email == null || string.IsNullOrEmpty(dto.Email))
                    updatedUser.Email = getUser.Email;
                if (dto.Role == null)
                    updatedUser.Role = getUser.Role;
                await unitOfWork.Users.UpdateUserAsync(updatedUser);
                return Results.Ok(new { message = "User updated successfully." });
            }).RequireAuthorization("Admin");
            #endregion

            #region Admin:Delete User
            app.MapDelete("/users/{id:int}", async (
                int id,
                IUnitOfWork unitOfWork
            ) =>
            {
                var (success, message) = await unitOfWork.Users.DeleteUserAsync(id);
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

        }

    }
}