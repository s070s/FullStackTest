using System.Text.RegularExpressions;

namespace Api.Services
{
    public interface IValidationService
    {
        bool IsValidEmail(string email);
        bool IsValidPassword(string password);
    }

    public class ValidationService : IValidationService
    {
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Simple email regex
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            // Example: min 8 chars, at least one letter and one number
            var pattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
            return Regex.IsMatch(password, pattern);
        }
    }
}