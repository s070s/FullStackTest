using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record WorkoutExerciseSetDto(
        int Id,
        int WorkoutExerciseId,
        WorkoutExercise WorkoutExercise,
        int SetNumber,
        int Repetitions,
        double? Weight,
        GoalUnit? GoalUnit,
        IntensityLevel OverallIntensityLevel,
        int DurationInSeconds,
        int RestPeriodInSeconds,
        string? Notes
    );

    public record CreateWorkoutExerciseSetDto(
        int WorkoutExerciseId,
        int SetNumber,
        int Repetitions,
        double? Weight,
        GoalUnit? GoalUnit,
        IntensityLevel OverallIntensityLevel,
        int DurationInSeconds,
        int RestPeriodInSeconds,
        string? Notes = null
    );

    public record UpdateWorkoutExerciseSetDto(
        int Id,
        int? SetNumber = null,
        int? Repetitions = null,
        double? Weight = null,
        GoalUnit? GoalUnit = null,
        IntensityLevel? OverallIntensityLevel = null,
        int? DurationInSeconds = null,
        int? RestPeriodInSeconds = null,
        string? Notes = null
    );
}