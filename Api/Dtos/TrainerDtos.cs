using Api.Models;
using Api.Models.Enums;
namespace Api.Dtos
{
    public record TrainerDto(
        int Id,
        int UserId,
        string? Bio,
        string? FirstName,
        string? LastName,
        DateTime? DateOfBirth,
        string? PhoneNumber,
        string? Address,
        string? City,
        string? State,
        string? ZipCode,
        string? Country,
        double? Weight,
        double? Height,
        double? BMR,
        double? BMI,
        int? Age,
        ICollection<ClientDto> Clients,
        ICollection<TrainerSpecialization> Specializations,
        ICollection<WorkoutDto> Workouts
    );

    public record CreateTrainerDto(
        int UserId,
        string? Bio = null,
        ICollection<TrainerSpecialization>? Specializations = null
    );

    public record UpdateTrainerDto(
        int Id,
        string? Bio = null,
        ICollection<TrainerSpecialization>? Specializations = null
    );
}