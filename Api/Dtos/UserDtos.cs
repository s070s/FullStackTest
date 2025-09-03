using Api.Models;

namespace Api.Dtos
{
	public record CreateUserDto(string Username, string Email, UserRole Role);
	public record UpdateUserDto(string? Email, bool? IsActive, UserRole Role);
	public record UserDto(int Id, string Username, string Email, DateTime CreatedUtc, bool IsActive, UserRole Role);
	public record RegisterUserDto(string Username, string Email, string Password, string Role);
	public record LoginDto(string Username, string Password);
}
