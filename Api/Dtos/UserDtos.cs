using System.Text.Json.Serialization;
using Api.Models.Enums;
using Api.Models;
using System.Globalization;

namespace Api.Dtos
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("role")]
        public UserRole Role { get; set; }

        [JsonPropertyName("profilePhotoUrl")]
        public string? ProfilePhotoUrl { get; set; }

        [JsonPropertyName("trainerProfile")]
        public TrainerDto? TrainerProfile { get; set; }

        [JsonPropertyName("clientProfile")]
        public ClientDto? ClientProfile { get; set; }

        [JsonPropertyName("createdUtcFormatted")]
        public string CreatedUtcFormatted => CreatedUtc.ToString("MMMM d, yyyy, h:mm tt", CultureInfo.InvariantCulture);
    }

    public class CreateUserDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("role")]
        public UserRole Role { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;

        [JsonPropertyName("profilePhotoUrl")]
        public string? ProfilePhotoUrl { get; set; }
    }

    public class UpdateUserDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("role")]
        public UserRole? Role { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    public class RegisterUserDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;
    }

    public class LoginDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;
    }

    public class UserStatisticsDto
    {
        [JsonPropertyName("totalUsers")]
        public int TotalUsers { get; set; }

        [JsonPropertyName("activeUsers")]
        public int ActiveUsers { get; set; }

        [JsonPropertyName("inactiveUsers")]
        public int InactiveUsers { get; set; }

        [JsonPropertyName("admins")]
        public int Admins { get; set; }

        [JsonPropertyName("trainers")]
        public int Trainers { get; set; }

        [JsonPropertyName("clients")]
        public int Clients { get; set; }
    }
}