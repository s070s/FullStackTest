using System.Text.RegularExpressions;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IValidationService
    {
        /// <summary>
        /// Validates if the provided email is in a correct format.
        /// </summary>
        bool IsValidEmail(string email);

        /// <summary>
        /// Validates if the password meets minimum security requirements.
        /// </summary>
        bool IsValidPassword(string password);

        /// <summary>
        /// Validates if the uploaded file is an allowed image type and size.
        /// </summary>
        bool IsValidImage(IFormFile file);

        /// <summary>
        /// Checks if a user with the given username or email already exists.
        /// </summary>
        Task<bool> UserExistsAsync(string username, string email);

        /// <summary>
        /// Determines if a user can be assigned a profile (must not already have one).
        /// </summary>
        Task<bool> CanAssignProfile(int userId);

        /// <summary>
        /// Checks if a user is active by username.
        /// </summary>
        Task<bool> IsUserActiveAsync(string username);
    }

    public class ValidationService : IValidationService
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Injects the application's database context.
        /// </summary>
        public ValidationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Simple email validation using regex pattern for basic format checking
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Simple email regex
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        // Password must be at least 8 characters, contain at least one letter and one number
        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            // Example: min 8 chars, at least one letter and one number
            var pattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
            return Regex.IsMatch(password, pattern);
        }
        // Checks if the user does not already have a Trainer or Client profile assigned
        public async Task<bool> CanAssignProfile(int userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.TrainerProfile)
                .Include(u => u.ClientProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user != null && user.TrainerProfile == null && user.ClientProfile == null;
        }
        // Check if a user with the given username or email already exists
        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username == username || u.Email == email);
        }


        // Check if a user is active by username
        public async Task<bool> IsUserActiveAsync(string username)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
            return user != null && user.IsActive;
        }



        // Only allow JPEG and PNG images up to 2MB for security and performance reasons
        public bool IsValidImage(IFormFile file)
        {
            // Allow only JPEG and PNG, max 2MB
            var allowedTypes = new[] { "image/jpeg", "image/png" };
            const long maxSize = 2 * 1024 * 1024; // 2MB

            if (file == null) return false;
            if (!allowedTypes.Contains(file.ContentType)) return false;
            if (file.Length > maxSize) return false;

            return true;
        }

    }
}