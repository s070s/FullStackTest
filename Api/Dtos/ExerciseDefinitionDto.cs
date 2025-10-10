using System.Text.Json.Serialization;
using Api.Models.Enums;

namespace Api.Dtos
{
    public class ExerciseDefinitionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("videoUrl")]
        public string? VideoUrl { get; set; }

        [JsonPropertyName("caloriesBurnedPerHour")]
        public int CaloriesBurnedPerHour { get; set; }

        [JsonPropertyName("equipments")]
        public ICollection<EquipmentDto>? Equipments { get; set; }

        [JsonPropertyName("isCompoundExercise")]
        public bool IsCompoundExercise { get; set; }

        [JsonPropertyName("primaryMuscleGroups")]
        public ICollection<MuscleGroup> PrimaryMuscleGroups { get; set; } = new List<MuscleGroup>();

        [JsonPropertyName("secondaryMuscleGroups")]
        public ICollection<MuscleGroup>? SecondaryMuscleGroups { get; set; }

        [JsonPropertyName("experienceLevel")]
        public ClientExperience? ExperienceLevel { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }
    }

    public class CreateExerciseDefinitionDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("videoUrl")]
        public string? VideoUrl { get; set; }

        [JsonPropertyName("caloriesBurnedPerHour")]
        public int CaloriesBurnedPerHour { get; set; } = 0;

        [JsonPropertyName("equipmentIds")]
        public ICollection<int>? EquipmentIds { get; set; }

        [JsonPropertyName("isCompoundExercise")]
        public bool IsCompoundExercise { get; set; } = false;

        [JsonPropertyName("primaryMuscleGroups")]
        public ICollection<MuscleGroup>? PrimaryMuscleGroups { get; set; }

        [JsonPropertyName("secondaryMuscleGroups")]
        public ICollection<MuscleGroup>? SecondaryMuscleGroups { get; set; }

        [JsonPropertyName("experienceLevel")]
        public ClientExperience? ExperienceLevel { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }
    }

    public class UpdateExerciseDefinitionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("videoUrl")]
        public string? VideoUrl { get; set; }

        [JsonPropertyName("caloriesBurnedPerHour")]
        public int? CaloriesBurnedPerHour { get; set; }

        [JsonPropertyName("equipmentIds")]
        public ICollection<int>? EquipmentIds { get; set; }

        [JsonPropertyName("isCompoundExercise")]
        public bool? IsCompoundExercise { get; set; }

        [JsonPropertyName("primaryMuscleGroups")]
        public ICollection<MuscleGroup>? PrimaryMuscleGroups { get; set; }

        [JsonPropertyName("secondaryMuscleGroups")]
        public ICollection<MuscleGroup>? SecondaryMuscleGroups { get; set; }

        [JsonPropertyName("experienceLevel")]
        public ClientExperience? ExperienceLevel { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }
    }
}