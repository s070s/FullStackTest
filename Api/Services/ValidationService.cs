using System.Text.RegularExpressions;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IValidationService
    {
        bool IsValidEmail(string email);
        bool IsValidPassword(string password);
        bool IsValidCount(int count, int min, int max);


        Task<bool> UserExistsAsync(string username, string email);

        Task<bool> CanAssignProfile(int userId);
    }

    public class ValidationService : IValidationService
    {
        private readonly AppDbContext _dbContext;

        public ValidationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Simple email validation using regex
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Simple email regex
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        // Simple password validation: at least 8 chars, one letter, one number
        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            // Example: min 8 chars, at least one letter and one number
            var pattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
            return Regex.IsMatch(password, pattern);
        }
        //Use in Endpoints before assigning TrainerProfile or ClientProfile to a User
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
        // Validate if a count is within a specified range ex.Trainer Specializations
        public bool IsValidCount(int count, int min, int max)
        {
            return count >= min && count <= max;
        }

    }
}