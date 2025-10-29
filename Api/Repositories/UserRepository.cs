using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;
using Api.Services.Mapping;
using Api.Models.Enums;

namespace Api.Repositories
{

    #region Interface
    /// <summary>
    /// Repository for managing User entities and related operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Returns user with both client and trainer profiles (if any).
        /// </summary>
        Task<UserDto?> GetUserWithProfilesByIdAsync(int userId);

        /// <summary>
        /// Returns user basic info (no profiles).
        /// </summary>
        Task<UserDto?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Returns User entity by username (null if not found).
        /// </summary>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Returns User entity by email (null if not found).
        /// </summary>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Returns paged list of users with total count, supports sorting and pagination.
        /// </summary>
        Task<(IEnumerable<UserDto> users, int total)> GetUsersPagedAsync(IPaginationService paginationService, int? page, int? pageSize, string? sortBy, string? sortOrder);

        /// <summary>
        /// Returns statistics about users (counts by role, active/inactive, etc).
        /// </summary>
        Task<UserStatisticsDto> GetUserStatisticsAsync();

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        Task AddUserAsync(User user);

        /// <summary>
        /// Updates an existing user and handles profile switching logic.
        /// </summary>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user and their profiles (if any).
        /// </summary>
        Task<(bool success, string? message)> DeleteUserAsync(int userId);

        /// <summary>
        /// Checks if a user exists by username or email.
        /// </summary>
        Task<bool> UserExistsAsync(string username, string email);

        /// <summary>
        /// Checks if a username exists for another user (for update scenarios).
        /// </summary>
        Task<bool> UsernameExistsAsync(string username, int? userId = null);

        /// <summary>
        /// Checks if an email exists in the system.
        /// </summary>
        Task<bool> EmailExistsAsync(string email, int? userId = null);

        /// <summary>
        /// Validates user credentials using BCrypt.
        /// </summary>
        Task<bool> ValidateUserCredentialsAsync(string username, string password);

        /// <summary>
        /// Checks if a user is active by username.
        /// </summary>
        Task<bool> IsUserActiveAsync(string username);

        /// <summary>
        /// Handles uploading and replacing user profile photo, returns new photo URL.
        /// </summary>
        Task<(bool success, string? message, string? photoUrl)> UploadUserProfilePhotoAsync(int userId, IFormFile file, IWebHostEnvironment env);
    }
    #endregion
    #region Implementation
    /// <inheritdoc/>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        private readonly IModelToDtoMapper _mapper;

        public UserRepository(AppDbContext db, IModelToDtoMapper mapper)
        {
            _db = db;
            _mapper = mapper;

        }

        /// <inheritdoc />
        public async Task<(bool success, string? message, string? photoUrl)> UploadUserProfilePhotoAsync(int userId, IFormFile file, IWebHostEnvironment env)
        {
            if (file == null || file.Length == 0)
                return (false, "No file uploaded.", null);

            if (env == null)
                throw new ArgumentNullException(nameof(env));

            var user = await _db.Users
                .Include(u => u.ClientProfile)
                .Include(u => u.TrainerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);
            string oldProfilePhotoUrl;
            string newProfilePhotoUrl;
            if (user == null)
                return (false, "User not found.", null);

            if (user.Role == UserRole.Admin)
                return (false, "Admins cannot have profile photos.", null);
            if (user.Role == UserRole.Client && user.ClientProfile != null)
            {
                oldProfilePhotoUrl = user.ClientProfile.ProfilePhotoUrl ?? "";
            }
            else if (user.Role == UserRole.Trainer && user.TrainerProfile != null)
            {
                oldProfilePhotoUrl = user.TrainerProfile.ProfilePhotoUrl ?? "";
            }
            else
            {
                return (false, "User must have a profile to upload a photo.", null);
            }
            // Delete old photo if it exists
            if (!string.IsNullOrEmpty(oldProfilePhotoUrl))
            {
                var oldFilePath = Path.Combine(env.WebRootPath ?? "wwwroot", oldProfilePhotoUrl.TrimStart('/', '\\'));
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

            if (user.Role == UserRole.Client && user.ClientProfile != null)
            {
                user.ClientProfile.ProfilePhotoUrl = $"/uploads/{newFileName}";
                newProfilePhotoUrl = user.ClientProfile.ProfilePhotoUrl;
                await _db.SaveChangesAsync();

                return (true, null, newProfilePhotoUrl);
            }

            else if (user.Role == UserRole.Trainer && user.TrainerProfile != null)
            {
                user.TrainerProfile.ProfilePhotoUrl = $"/uploads/{newFileName}";
                newProfilePhotoUrl = user.TrainerProfile.ProfilePhotoUrl;
                await _db.SaveChangesAsync();

                return (true, null, newProfilePhotoUrl);
            }
            return (false, "Failed to update profile photo.", null);
        }

        /// <inheritdoc />
        public async Task<UserDto?> GetUserWithProfilesByIdAsync(int userId)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Include(u => u.TrainerProfile)
                .Include(u => u.ClientProfile)
                .SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            TrainerDto? trainerProfile = _mapper.ToTrainerDto(user.TrainerProfile);
            ClientDto? clientProfile = _mapper.ToClientDto(user.ClientProfile);

            return new UserDto
            {
                Id = user.Id,
                CreatedUtc = user.CreatedUtc,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role,
                TrainerProfile = trainerProfile,
                ClientProfile = clientProfile
            };
        }

        /// <inheritdoc />
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
                TrainerProfile = null,
                ClientProfile = null
            };
        }


        /// <inheritdoc />
        public async Task<(IEnumerable<UserDto> users, int total)> GetUsersPagedAsync(IPaginationService paginationService, int? page, int? pageSize, string? sortBy, string? sortOrder)
        {
            if (paginationService == null)
                throw new ArgumentNullException(nameof(paginationService));

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
                    TrainerProfile = u.TrainerProfile != null ? _mapper.ToTrainerDto(u.TrainerProfile) : null,
                    ClientProfile = u.ClientProfile != null ? _mapper.ToClientDto(u.ClientProfile) : null
                })
                .ToListAsync();
            return (users, total);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task AddUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<(bool success, string? message)> DeleteUserAsync(int userId)
        {
            var user = await _db.Users.Include(u => u.ClientProfile).Include(u => u.TrainerProfile).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return (false, "User not found.");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        /// <inheritdoc />
        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

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

        /// <inheritdoc />
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc />
        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _db.Users.AnyAsync(u => u.Username == username || u.Email == email);
        }

        /// <inheritdoc />
        public async Task<bool> UsernameExistsAsync(string username, int? userId = null)
        {
            return await _db.Users.AnyAsync(u => u.Username == username && (!userId.HasValue || u.Id != userId.Value));
        }

        /// <inheritdoc />
        public async Task<bool> EmailExistsAsync(string email, int? userId = null)
        {
            return await _db.Users.AnyAsync(u => u.Email == email && (!userId.HasValue || u.Id != userId.Value));
        }

        /// <inheritdoc />
        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                return false;
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        /// <inheritdoc />
        public async Task<bool> IsUserActiveAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user != null && user.IsActive;
        }
    }
    #endregion

}