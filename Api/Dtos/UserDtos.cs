using Api.Models.Enums;

namespace Api.Dtos
{

	// For returning user data
	public record UserDto(
		int Id,
		string Username,
		string Email,
		DateTime CreatedUtc,
		bool IsActive,
		UserRole Role,
		string? ProfilePhotoUrl
	);
	// For creating a user, all fields except Id and CreatedUtc are required
	public record CreateUserDto(
		string Username,
		string Email,
		UserRole Role,
		bool IsActive = true,
		string? ProfilePhotoUrl = null
	);

	// For updating a user, all fields are optional except Id,UserRole defaults to Client if not provided
	public record UpdateUserDto(
		int Id,
		string? Email = null,
		bool? IsActive = null,
		UserRole Role = UserRole.Client,
		string? ProfilePhotoUrl = null
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
