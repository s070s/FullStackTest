using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record ExerciseDefinitionDto(
        int Id,
        string? Name,
        string? Description,
        string? VideoUrl,
        int CaloriesBurnedPerHour,
        ICollection<EquipmentDto>? Equipments,
        bool IsCompoundExercise,
        ICollection<MuscleGroup> PrimaryMuscleGroups,
        ICollection<MuscleGroup>? SecondaryMuscleGroups,
        ClientExperience? ExperienceLevel,
        string? Category
    );

    public record CreateExerciseDefinitionDto(
        string? Name,
        string? Description = null,
        string? VideoUrl = null,
        int CaloriesBurnedPerHour = 0,
        ICollection<int>? EquipmentIds = null,
        bool IsCompoundExercise = false,
        ICollection<MuscleGroup>? PrimaryMuscleGroups = null,
        ICollection<MuscleGroup>? SecondaryMuscleGroups = null,
        ClientExperience? ExperienceLevel = null,
        string? Category = null
    );

    public record UpdateExerciseDefinitionDto(
        int Id,
        string? Name = null,
        string? Description = null,
        string? VideoUrl = null,
        int? CaloriesBurnedPerHour = null,
        ICollection<int>? EquipmentIds = null,
        bool? IsCompoundExercise = null,
        ICollection<MuscleGroup>? PrimaryMuscleGroups = null,
        ICollection<MuscleGroup>? SecondaryMuscleGroups = null,
        ClientExperience? ExperienceLevel = null,
        string? Category = null
    );
}