using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
        public class ExerciseDefinition : BaseEntity
        {
                [Required]
                [StringLength(100)]
                public string? Name { get; set; }

                [StringLength(1000)]
                public string? Description { get; set; }

                [Url]
                public string? VideoUrl { get; set; }

                [Range(0, 2000)]
                public int CaloriesBurnedPerHour { get; set; }

                public ICollection<Equipment>? Equipments { get; set; } = new List<Equipment>();

                public bool IsCompoundExercise { get; set; }

                [MinLength(1)]
                public ICollection<MuscleGroup> PrimaryMuscleGroups { get; set; } = new List<MuscleGroup>();

                public ICollection<MuscleGroup>? SecondaryMuscleGroups { get; set; } = new List<MuscleGroup>();

                public ClientExperience? ExperienceLevel { get; set; }
                public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
                
                [StringLength(50)]
                public string? Category { get; set; }
        }
}