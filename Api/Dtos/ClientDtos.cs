using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record ClientDto(
        int Id,
        int UserId,
        string? Bio,
        ClientExperience ExperienceLevel,
        MedicalHistoryDto? MedicalHistory,
        IntensityLevel PreferredIntensityLevel,
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
        ICollection<GoalDto> Goals,
        ICollection<WorkoutDto> Workouts,
        ICollection<TrainerDto> Trainers,
        ICollection<MeasurementDto> Measurements
    );

    public record CreateClientDto(
        int UserId,
        string? Bio = null,
        ClientExperience ExperienceLevel = ClientExperience.Beginner,
        MedicalHistoryDto? MedicalHistory = null,
        IntensityLevel PreferredIntensityLevel = IntensityLevel.Medium
    );

    public record UpdateClientDto(
        int Id,
        string? Bio = null,
        ClientExperience? ExperienceLevel = null,
        MedicalHistoryDto? MedicalHistory = null,
        IntensityLevel? PreferredIntensityLevel = null
    );
}