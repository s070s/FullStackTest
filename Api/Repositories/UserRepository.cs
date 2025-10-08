using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;
using Api.Services;
using Api.Models.Enums;
using Api.Mappings;

namespace Api.Repositories
{
    public interface IUserRepository
    {
        // Returns user with both client and trainer profiles (if any)
        Task<UserDto?> GetUserWithProfilesByIdAsync(int userId);

        // Returns user basic info (no profiles)
        Task<UserDto?> GetUserByIdAsync(int userId);

        // Returns User entity by username (null if not found)
        Task<User?> GetUserByUsernameAsync(string username);

        // Returns User entity by email (null if not found)
        Task<User?> GetUserByEmailAsync(string email);

        // Returns paged list of users with total count, supports sorting and pagination
        Task<(IEnumerable<UserDto> users, int total)> GetUsersPagedAsync(IPaginationService paginationService, int? page, int? pageSize, string? sortBy, string? sortOrder);

        // Returns statistics about users (counts by role, active/inactive, etc)
        Task<UserStatisticsDto> GetUserStatisticsAsync();

        // Adds a new user to the database
        Task AddUserAsync(User user);

        // Updates an existing user and handles profile switching logic
        Task UpdateUserAsync(User user);

        // Deletes a user and their profiles (if any)
        Task<(bool success, string? message)> DeleteUserAsync(int userId);

        // Checks if a user exists by username or email
        Task<bool> UserExistsAsync(string username, string email);

        // Checks if a username exists for another user (for update scenarios)
        Task<bool> UsernameExistsAsync(string username, int? userId = null);

        // Checks if an email exists in the system
        Task<bool> EmailExistsAsync(string email, int? userId = null);

        // Validates user credentials using BCrypt
        Task<bool> ValidateUserCredentialsAsync(string username, string password);

        // Checks if a user is active by username
        Task<bool> IsUserActiveAsync(string username);

        // Switches user role between Client and Trainer, handles profile changes
        Task<(bool success, string? message, UserRole? newRole)> SwitchUserRoleAsync(int userId, IClientRepository clientRepository, ITrainerRepository trainerRepository);

        // Assigns a profile (Client or Trainer) to a user, validates assignment
        Task<(bool success, string? message)> AssignProfileAsync(int userId, UserRole role, IClientRepository clientRepository, ITrainerRepository trainerRepository, IValidationService validator);

        // Handles uploading and replacing user profile photo, returns new photo URL
        Task<(bool success, string? message, string? photoUrl)> UploadUserProfilePhotoAsync(int userId, IFormFile file, IWebHostEnvironment env);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        // Uploads a new profile photo for the user, deletes old photo if exists
        public async Task<(bool success, string? message, string? photoUrl)> UploadUserProfilePhotoAsync(int userId, IFormFile file, IWebHostEnvironment env)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found.", null);

            // Delete old photo if it exists
            if (!string.IsNullOrEmpty(user.ProfilePhotoUrl))
            {
                var oldFilePath = Path.Combine(env.WebRootPath ?? "wwwroot", user.ProfilePhotoUrl.TrimStart('/', '\\'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            var uploadsDir = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileExtension = Path.GetExtension(file.FileName);
            var newFileName = $"user_{userId}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsDir, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfilePhotoUrl = $"/uploads/{newFileName}";
            await _db.SaveChangesAsync();

            return (true, null, user.ProfilePhotoUrl);
        }

        // Gets user with both profiles (if any), returns as UserDto
        public async Task<UserDto?> GetUserWithProfilesByIdAsync(int userId)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Include(u => u.TrainerProfile)
                .Include(u => u.ClientProfile)
                .SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

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

            return new UserDto
            {
                Id = user.Id,
                CreatedUtc = user.CreatedUtc,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                TrainerProfile = trainerProfile,
                ClientProfile = clientProfile
            };
        }

        // Gets user by ID, returns as UserDto (no profiles)
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _db.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                CreatedUtc = user.CreatedUtc,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                TrainerProfile = null,
                ClientProfile = null
            };
        }

        // Assigns a profile (Client/Trainer) to a user, validates assignment
        public async Task<(bool success, string? message)> AssignProfileAsync(int userId, UserRole role, IClientRepository clientRepository, ITrainerRepository trainerRepository, IValidationService validator)
        {
            var user = await _db.Users.Include(u => u.ClientProfile).Include(u => u.TrainerProfile).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return (false, "User not found.");
            if (user.Role != role)
                return (false, "User role does not match the profile to be assigned.");
            if (!await validator.CanAssignProfile(userId))
                return (false, "User already has a profile assigned.");
            if (role == UserRole.Client)
            {
                var client = new Client
                {
                    UserId = user.Id,
                    User = user
                };
                await clientRepository.AddClientAsync(client);
                return (true, "Client profile assigned successfully.");
            }
            else if (role == UserRole.Trainer)
            {
                var trainer = new Trainer
                {
                    UserId = user.Id,
                    User = user
                };
                await trainerRepository.AddTrainerAsync(trainer);
                return (true, "Trainer profile assigned successfully.");
            }
            return (false, "Invalid role.");
        }

        // Switches user role between Client and Trainer, updates profiles accordingly
        public async Task<(bool success, string? message, UserRole? newRole)> SwitchUserRoleAsync(int userId, IClientRepository clientRepository, ITrainerRepository trainerRepository)
        {
            var user = await _db.Users.Include(u => u.ClientProfile).Include(u => u.TrainerProfile).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return (false, "User not found.", null);
            if (user.Role == UserRole.Admin)
                return (false, "Cannot change role of an Admin.", null);

            if (user.Role == UserRole.Client)
            {
                // Switch to Trainer
                if (user.ClientProfile != null)
                {
                    _db.Clients.Remove(user.ClientProfile);
                    user.ClientProfile = null;
                }
                var trainer = new Trainer
                {
                    UserId = user.Id,
                    User = user
                };
                await trainerRepository.AddTrainerAsync(trainer);
                user.Role = UserRole.Trainer;
            }
            else if (user.Role == UserRole.Trainer)
            {
                // Switch to Client
                if (user.TrainerProfile != null)
                {
                    _db.Trainers.Remove(user.TrainerProfile);
                    user.TrainerProfile = null;
                }
                var client = new Client
                {
                    UserId = user.Id,
                    User = user
                };
                await clientRepository.AddClientAsync(client);
                user.Role = UserRole.Client;
            }
            await _db.SaveChangesAsync();
            return (true, null, user.Role);
        }

        // Returns paged and sorted list of users with profiles, and total count
        public async Task<(IEnumerable<UserDto> users, int total)> GetUsersPagedAsync(IPaginationService paginationService, int? page, int? pageSize, string? sortBy, string? sortOrder)
        {
            var query = _db.Users
                .AsNoTracking()
                .Include(u => u.TrainerProfile)
                .Include(u => u.ClientProfile) as IQueryable<User>;
            var total = await query.CountAsync();
            query = paginationService.ApplySorting(query, sortBy, sortOrder);
            query = paginationService.ApplyPagination(query, page, pageSize);
            var users = await query
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    CreatedUtc = u.CreatedUtc,
                    Username = u.Username,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    Role = u.Role,
                    ProfilePhotoUrl = u.ProfilePhotoUrl,
                    TrainerProfile = u.TrainerProfile != null ? u.TrainerProfile.ToTrainerDto() : null,
                    ClientProfile = u.ClientProfile != null ? u.ClientProfile.ToClientDto() : null
                })
                .ToListAsync();
            return (users, total);
        }

        // Returns statistics about users (counts by role, active/inactive, etc)
        public async Task<UserStatisticsDto> GetUserStatisticsAsync()
        {
            var totalUsers = await _db.Users.CountAsync();
            var activeUsers = await _db.Users.CountAsync(u => u.IsActive);
            var inactiveUsers = totalUsers - activeUsers;
            var totalAdmins = await _db.Users.CountAsync(u => u.Role == UserRole.Admin);
            var totalTrainers = await _db.Users.CountAsync(u => u.Role == UserRole.Trainer);
            var totalClients = await _db.Users.CountAsync(u => u.Role == UserRole.Client);
            var stats = new UserStatisticsDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,
                Admins = totalAdmins,
                Trainers = totalTrainers,
                Clients = totalClients
            };
            return stats;
        }

        // Adds a new user to the database
        public async Task AddUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        // Deletes a user and their profiles (if any)
        public async Task<(bool success, string? message)> DeleteUserAsync(int userId)
        {
            var user = await _db.Users.Include(u => u.ClientProfile).Include(u => u.TrainerProfile).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return (false, "User not found.");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        // Updates an existing user, handles profile switching logic
        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _db.Users
                .Include(u => u.ClientProfile)
                .Include(u => u.TrainerProfile)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null) return;

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.IsActive = user.IsActive;
            existingUser.Role = user.Role;
            if (!string.IsNullOrEmpty(user.PasswordHash))
                existingUser.PasswordHash = user.PasswordHash;

            // Handle profile switching
            if (user.Role == UserRole.Client)
            {
                // Remove TrainerProfile if exists
                if (existingUser.TrainerProfile != null)
                {
                    _db.Trainers.Remove(existingUser.TrainerProfile);
                    existingUser.TrainerProfile = null;
                }
                // Assign ClientProfile if not exists
                if (existingUser.ClientProfile == null)
                {
                    var client = new Client
                    {
                        UserId = existingUser.Id,
                        User = existingUser
                    };
                    _db.Clients.Add(client);
                    existingUser.ClientProfile = client;
                }
            }
            else if (user.Role == UserRole.Trainer)
            {
                // Remove ClientProfile if exists
                if (existingUser.ClientProfile != null)
                {
                    _db.Clients.Remove(existingUser.ClientProfile);
                    existingUser.ClientProfile = null;
                }
                // Assign TrainerProfile if not exists
                if (existingUser.TrainerProfile == null)
                {
                    var trainer = new Trainer
                    {
                        UserId = existingUser.Id,
                        User = existingUser
                    };
                    _db.Trainers.Add(trainer);
                    existingUser.TrainerProfile = trainer;
                }
            }
            else if (user.Role == UserRole.Admin)
            {
                // Remove both profiles if exist
                if (existingUser.ClientProfile != null)
                {
                    _db.Clients.Remove(existingUser.ClientProfile);
                    existingUser.ClientProfile = null;
                }
                if (existingUser.TrainerProfile != null)
                {
                    _db.Trainers.Remove(existingUser.TrainerProfile);
                    existingUser.TrainerProfile = null;
                }
            }
            await _db.SaveChangesAsync();
        }

        // Gets user by username (returns User entity)
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        // Gets user by email (returns User entity)
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        // Checks if a user exists by username or email
        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _db.Users.AnyAsync(u => u.Username == username || u.Email == email);
        }

        // Checks if a username exists for another user (for update scenarios)
        public async Task<bool> UsernameExistsAsync(string username, int? userId = null)
        {
            return await _db.Users.AnyAsync(u => u.Username == username && (!userId.HasValue || u.Id != userId.Value));
        }

        // Checks if an email exists in the system
        public async Task<bool> EmailExistsAsync(string email, int? userId = null)
        {
            return await _db.Users.AnyAsync(u => u.Email == email && (!userId.HasValue || u.Id != userId.Value));
        }

        // Validates user credentials using BCrypt
        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        // Checks if a user is active by username
        public async Task<bool> IsUserActiveAsync(string username)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user != null && user.IsActive;
        }
    }
}