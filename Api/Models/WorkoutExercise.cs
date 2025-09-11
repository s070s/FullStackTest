using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

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


        public WorkoutExerciseDto ToWorkoutExerciseDto()
        {
            return new WorkoutExerciseDto(
                Id,
                WorkoutId,
                Workout,
                ExerciseDefinitionId,
                ExerciseDefinition,
                Sets,
                Notes
            );
        }

        public CreateWorkoutExerciseDto ToCreateWorkoutExerciseDto()
        {
            return new CreateWorkoutExerciseDto(
                WorkoutId,
                ExerciseDefinitionId,
                Notes
            );
        }

        public UpdateWorkoutExerciseDto ToUpdateWorkoutExerciseDto()
        {
            return new UpdateWorkoutExerciseDto(
                Id,
                Notes
            );
        }

    }
}