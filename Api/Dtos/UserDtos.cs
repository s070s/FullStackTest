using Api.Models.Enums;
using Api.Models;

namespace Api.Dtos
{
    // For returning user data
    public record UserDto(
        int Id,
        string Username,
        string Email,
        bool IsActive,
        UserRole Role,
        string? ProfilePhotoUrl,
        Trainer? TrainerProfile,
        Client? ClientProfile
    );

    // For creating a user, all fields except Id are required
    public record CreateUserDto(
        string Username,
        string Email,
        UserRole Role,
        string Password,
        bool IsActive = true,
        string? ProfilePhotoUrl = null
    );

    // For updating a user, all fields are optional except Id
    public record UpdateUserDto(
        int Id,
        string? Username = null,
        string? Email = null,
        bool? IsActive = null,
        UserRole? Role = null,
        string? ProfilePhotoUrl = null,
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
}