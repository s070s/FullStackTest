using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record ClientDto(
        int Id,
        int UserId,
        UserDto User,
        string? Bio,
        ClientExperience ExperienceLevel,
        MedicalHistory? MedicalHistory,
        IntensityLevel PreferredIntensityLevel,
        ICollection<Goal> Goals,
        ICollection<Workout> Workouts,
        ICollection<Trainer> Trainers,
        ICollection<Measurement> Measurements
    );

    public record CreateClientDto(
        int UserId,
        string? Bio = null,
        ClientExperience ExperienceLevel = ClientExperience.Beginner,
        MedicalHistory? MedicalHistory = null,
        IntensityLevel PreferredIntensityLevel = IntensityLevel.Medium
    );

    public record UpdateClientDto(
        int Id,
        string? Bio = null,
        ClientExperience? ExperienceLevel = null,
        MedicalHistory? MedicalHistory = null,
        IntensityLevel? PreferredIntensityLevel = null
    );
}