using Api.Models.Enums;
using Api.Models;
using System.Globalization; // Add this

namespace Api.Dtos
{
    // For returning user data
    public record UserDto(
        int Id,
        DateTime CreatedUtc,
        string Username,
        string Email,
        bool IsActive,
        UserRole Role,
        string? ProfilePhotoUrl,
        TrainerDto? TrainerProfile,
        ClientDto? ClientProfile
    )
    {
        public string CreatedUtcFormatted => CreatedUtc.ToString("MMMM d, yyyy, h:mm tt", CultureInfo.InvariantCulture);
    }

    // For creating a user, all fields except Id are required
    public record CreateUserDto(
        string Username,
        string Email,
        UserRole Role,
        string Password,
        bool IsActive = true,
        string? ProfilePhotoUrl = null
    );

    public record UpdateUserDto(
        int Id,
        string? Username = null,
        string? Email = null,
        bool? IsActive = null,
        UserRole? Role = null,
        string? Password = null
    );

    public record RegisterUserDto(
        string Username,
        string Email,
        string Password,
        string Role
    );

    public record LoginDto(
        string Username,
        string Password
    );

    public record RefreshTokenRequestDto(
        string RefreshToken
    );

    public record TokenPairDto(
        string AccessToken,
        DateTime AccessTokenExpiresUtc,
        string RefreshToken,
        DateTime RefreshTokenExpiresUtc
    );

public class UserStatisticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int Admins { get; set; }
    public int Trainers { get; set; }
    public int Clients { get; set; }
}
}