using System.Text.Json.Serialization;
using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public class WorkoutExerciseSetDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("workoutExerciseId")]
        public int WorkoutExerciseId { get; set; }

        [JsonPropertyName("workoutExercise")]
        public WorkoutExerciseDto WorkoutExercise { get; set; } = null!;

        [JsonPropertyName("setNumber")]
        public int SetNumber { get; set; }

        [JsonPropertyName("repetitions")]
        public int Repetitions { get; set; }

        [JsonPropertyName("weight")]
        public double? Weight { get; set; }

        [JsonPropertyName("goalUnit")]
        public GoalUnit? GoalUnit { get; set; }

        [JsonPropertyName("overallIntensityLevel")]
        public IntensityLevel OverallIntensityLevel { get; set; }

        [JsonPropertyName("durationInSeconds")]
        public int DurationInSeconds { get; set; }

        [JsonPropertyName("restPeriodInSeconds")]
        public int RestPeriodInSeconds { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    public class CreateWorkoutExerciseSetDto
    {
        [JsonPropertyName("workoutExerciseId")]
        public int WorkoutExerciseId { get; set; }

        [JsonPropertyName("setNumber")]
        public int SetNumber { get; set; }

        [JsonPropertyName("repetitions")]
        public int Repetitions { get; set; }

        [JsonPropertyName("weight")]
        public double? Weight { get; set; }

        [JsonPropertyName("goalUnit")]
        public GoalUnit? GoalUnit { get; set; }

        [JsonPropertyName("overallIntensityLevel")]
        public IntensityLevel OverallIntensityLevel { get; set; }

        [JsonPropertyName("durationInSeconds")]
        public int DurationInSeconds { get; set; }

        [JsonPropertyName("restPeriodInSeconds")]
        public int RestPeriodInSeconds { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    public class UpdateWorkoutExerciseSetDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("setNumber")]
        public int? SetNumber { get; set; }

        [JsonPropertyName("repetitions")]
        public int? Repetitions { get; set; }

        [JsonPropertyName("weight")]
        public double? Weight { get; set; }

        [JsonPropertyName("goalUnit")]
        public GoalUnit? GoalUnit { get; set; }

        [JsonPropertyName("overallIntensityLevel")]
        public IntensityLevel? OverallIntensityLevel { get; set; }

        [JsonPropertyName("durationInSeconds")]
        public int? DurationInSeconds { get; set; }

        [JsonPropertyName("restPeriodInSeconds")]
        public int? RestPeriodInSeconds { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }
}