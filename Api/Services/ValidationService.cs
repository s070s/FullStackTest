using System.Text.RegularExpressions;
using Api.Data;
using Api.Dtos;
using Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    #region Interface
    /// <summary>
    /// Service for validating various user inputs and states.
    /// </summary>
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

        /// <summary>
        /// Validates that the userId claim matches the route id.
        /// </summary>
        bool IsUserIdClaimMatchingRouteId(HttpContext context, int routeId);

        /// <summary>
        /// Validates the client profile update data.
        /// </summary>
        bool IsValidClientProfileUpdate(UpdateClientProfileDto dto);

        /// <summary>
        /// Validates the trainer profile update data.
        /// </summary>
        bool IsValidTrainerProfileUpdate(UpdateTrainerProfileDto dto);
    }
    #endregion
    #region Implementation
    /// <inheritdoc />
    public class ValidationService : IValidationService
    {
        private readonly AppDbContext _dbContext;

        /// <inheritdoc />
        public ValidationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        /// <inheritdoc />
        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            var pattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        /// <inheritdoc />
        public async Task<bool> CanAssignProfile(int userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.TrainerProfile)
                .Include(u => u.ClientProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user != null && user.TrainerProfile == null && user.ClientProfile == null;
        }

        /// <inheritdoc />
        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username == username || u.Email == email);
        }

        /// <inheritdoc />
        public async Task<bool> IsUserActiveAsync(string username)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
            return user != null && user.IsActive;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool IsUserIdClaimMatchingRouteId(HttpContext context, int routeId)
        {
            var userIdClaim = context.User.FindFirst("userid")?.Value;
            if (userIdClaim == null) return false;
            return int.TryParse(userIdClaim, out int userId) && userId == routeId;
        }

        /// <inheritdoc />
        public bool IsValidClientProfileUpdate(UpdateClientProfileDto dto)
        {
            if (dto == null) return false;

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                return false;

            if (string.IsNullOrWhiteSpace(dto.LastName))
                return false;

            if (string.IsNullOrWhiteSpace(dto.Bio))
                return false;

            if (string.IsNullOrWhiteSpace(dto.Country))
                return false;

            if (string.IsNullOrWhiteSpace(dto.Address))
                return false;

            if (dto.ExperienceLevel.HasValue && !Enum.IsDefined(typeof(ClientExperience), dto.ExperienceLevel.Value))
                return false;

            return true;
        }
        /// <inheritdoc />
        public bool IsValidTrainerProfileUpdate(UpdateTrainerProfileDto dto)
        {
            if (dto == null) return false;

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                return false;

            if (string.IsNullOrWhiteSpace(dto.LastName))
                return false;

            if (string.IsNullOrWhiteSpace(dto.Bio))
                return false;

            if (string.IsNullOrWhiteSpace(dto.Country))
                return false;
            if (string.IsNullOrWhiteSpace(dto.Address))
                return false;
            return true;
        }
    }
    #endregion

}