using System.Text.Json.Serialization;

namespace Api.Dtos
{
    public class WorkoutExerciseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("workoutId")]
        public int WorkoutId { get; set; }

        [JsonPropertyName("workout")]
        public WorkoutDto Workout { get; set; } = null!;

        [JsonPropertyName("exerciseDefinitionId")]
        public int ExerciseDefinitionId { get; set; }

        [JsonPropertyName("exerciseDefinition")]
        public ExerciseDefinitionDto ExerciseDefinition { get; set; } = null!;

        [JsonPropertyName("sets")]
        public ICollection<WorkoutExerciseSetDto> Sets { get; set; } = new List<WorkoutExerciseSetDto>();

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    public class CreateWorkoutExerciseDto
    {
        [JsonPropertyName("workoutId")]
        public int WorkoutId { get; set; }

        [JsonPropertyName("exerciseDefinitionId")]
        public int ExerciseDefinitionId { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    public class UpdateWorkoutExerciseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }
}