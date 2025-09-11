using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record WorkoutExerciseDto(
        int Id,
        int WorkoutId,
        Workout Workout,
        int ExerciseDefinitionId,
        ExerciseDefinition ExerciseDefinition,
        ICollection<WorkoutExerciseSet> Sets,
        string? Notes
    );

    public record CreateWorkoutExerciseDto(
        int WorkoutId,
        int ExerciseDefinitionId,
        string? Notes = null
    );

    public record UpdateWorkoutExerciseDto(
        int Id,
        string? Notes = null
    );
}