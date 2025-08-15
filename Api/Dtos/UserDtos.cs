public record CreateUserDto(string Username, string Email);
public record UpdateUserDto(string? Email, bool? IsActive);
public record UserDto(int Id, string Username, string Email, DateTime CreatedUtc, bool IsActive);
