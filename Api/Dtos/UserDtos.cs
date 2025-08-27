namespace Api.Dtos
{
	public record CreateUserDto(string Username, string Email);
	public record UpdateUserDto(string? Email, bool? IsActive);
	public record UserDto(int Id, string Username, string Email, DateTime CreatedUtc, bool IsActive);
	public record RegisterUserDto(string Username, string Email, string Password);
	public record LoginDto(string Username, string Password);
}
