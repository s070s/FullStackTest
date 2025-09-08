namespace Api.Dtos
{
    public record TrainerDto(
        int Id,
        int UserId,
        UserDto User,
        string? Username,
        string? Bio,
        string? Specialization
    );
    public record CreateTrainerDto(
        int UserId,
        string? Bio = null,
        string? Specialization = null
    );
    public record UpdateTrainerDto(
        int Id,
        string? Bio = null,
        string? Specialization = null
    );
}

