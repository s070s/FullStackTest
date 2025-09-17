using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;
using Api.Services;
using Api.Models.Enums;


namespace Api.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto?> GetUserWithProfilesByIdAsync(int userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<(IEnumerable<UserDto> users, int total)> GetUsersPagedAsync(IPaginationService paginationService, int? page, int? pageSize, string? sortBy, string? sortOrder);
        Task AddUserAsync(User user);
        Task<(bool success, string? message)> DeleteUserAsync(int userId);
        Task<bool> UserExistsAsync(string username, string email);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<bool> IsUserActiveAsync(string username);
        Task<(bool success, string? message, UserRole? newRole)> SwitchUserRoleAsync(int userId, IClientRepository clientRepository, ITrainerRepository trainerRepository);
        Task<(bool success, string? message)> AssignProfileAsync(int userId, UserRole role, IClientRepository clientRepository, ITrainerRepository trainerRepository, IValidationService validator);
        Task<(bool success, string? message, string? photoUrl)> UploadUserProfilePhotoAsync(int userId, IFormFile file, IWebHostEnvironment env);
    }

    public class UserRepository : IUserRepository
    {



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

            return new UserDto(
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
        }
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

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
            return (users, total);
        }

        public async Task AddUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<(bool success, string? message)> DeleteUserAsync(int userId)
        {
            var user = await _db.Users.Include(u => u.ClientProfile).Include(u => u.TrainerProfile).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return (false, "User not found.");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _db.Users.AnyAsync(u => u.Username == username || u.Email == email);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<bool> IsUserActiveAsync(string username)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user != null && user.IsActive;
        }
    }
}