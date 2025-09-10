using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class WorkoutExercise : BaseEntity
    {
        [Required]
        public int WorkoutId { get; set; }

        [Required]
        public Workout Workout { get; set; } = null!;

        [Required]
        public int ExerciseDefinitionId { get; set; }

        [Required]
        public ExerciseDefinition ExerciseDefinition { get; set; } = null!;

        public ICollection<WorkoutExerciseSet> Sets { get; set; } = new List<WorkoutExerciseSet>();

        [MaxLength(500)]
        public string? Notes { get; set; }

    }
}