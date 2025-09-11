using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

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

                public ExerciseDefinitionDto ToExerciseDefinitionDto()
                {
                        return new ExerciseDefinitionDto(
                            Id,
                            Name,
                            Description,
                            VideoUrl,
                            CaloriesBurnedPerHour,
                            Equipments,
                            IsCompoundExercise,
                            PrimaryMuscleGroups,
                            SecondaryMuscleGroups,
                            ExperienceLevel,
                            Category
                        );
                }

                public CreateExerciseDefinitionDto ToCreateExerciseDefinitionDto()
                {
                        return new CreateExerciseDefinitionDto(
                            Name,
                            Description,
                            VideoUrl,
                            CaloriesBurnedPerHour,
                            Equipments?.Select(e => e.Id).ToList(),
                            IsCompoundExercise,
                            PrimaryMuscleGroups,
                            SecondaryMuscleGroups,
                            ExperienceLevel,
                            Category
                        );
                }

                public UpdateExerciseDefinitionDto ToUpdateExerciseDefinitionDto()
                {
                        return new UpdateExerciseDefinitionDto(
                            Id,
                            Name,
                            Description,
                            VideoUrl,
                            CaloriesBurnedPerHour,
                            Equipments?.Select(e => e.Id).ToList(),
                            IsCompoundExercise,
                            PrimaryMuscleGroups,
                            SecondaryMuscleGroups,
                            ExperienceLevel,
                            Category
                        );
                }
        }
}