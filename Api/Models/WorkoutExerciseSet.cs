using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

namespace Api.Models
{
    public class WorkoutExerciseSet : BaseEntity
    {
        [Required]
        public int WorkoutExerciseId { get; set; }

        [Required]
        public WorkoutExercise WorkoutExercise { get; set; } = null!;

        [Range(1, 20)]
        public int SetNumber { get; set; }

        [Range(1, 200)]
        public int Repetitions { get; set; }

        [Range(0, 1000)]
        public float? Weight { get; set; }

        public GoalUnit? GoalUnit { get; set; }

        [Required]
        public IntensityLevel OverallIntensityLevel { get; set; }

        [Range(0, int.MaxValue)]
        public int DurationInSeconds { get; set; }

        [Range(0, int.MaxValue)]
        public int RestPeriodInSeconds { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
        public WorkoutExerciseSetDto ToWorkoutExerciseSetDto()
        {
            return new WorkoutExerciseSetDto(
                Id,
                WorkoutExerciseId,
                WorkoutExercise,
                SetNumber,
                Repetitions,
                Weight,
                GoalUnit,
                OverallIntensityLevel,
                DurationInSeconds,
                RestPeriodInSeconds,
                Notes
            );
        }

        public CreateWorkoutExerciseSetDto ToCreateWorkoutExerciseSetDto()
        {
            return new CreateWorkoutExerciseSetDto(
                WorkoutExerciseId,
                SetNumber,
                Repetitions,
                Weight,
                GoalUnit,
                OverallIntensityLevel,
                DurationInSeconds,
                RestPeriodInSeconds,
                Notes
            );
        }

        public UpdateWorkoutExerciseSetDto ToUpdateWorkoutExerciseSetDto()
        {
            return new UpdateWorkoutExerciseSetDto(
                Id,
                SetNumber,
                Repetitions,
                Weight,
                GoalUnit,
                OverallIntensityLevel,
                DurationInSeconds,
                RestPeriodInSeconds,
                Notes
            );
        }
    }

}