using Api.Models;
using Api.Models.Enums;
namespace Api.Dtos
{
    public record TrainerDto(
        int Id,
        int UserId,
        string? Bio,
        ICollection<Client> Clients,
        ICollection<TrainerSpecialization> Specializations,
        ICollection<Workout> Workouts
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