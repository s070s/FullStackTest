using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

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
    }

}