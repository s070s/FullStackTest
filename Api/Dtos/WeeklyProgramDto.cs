using System.Text.Json.Serialization;

namespace Api.Dtos
{
    public class WeeklyProgramDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("durationInWeeks")]
        public int DurationInWeeks { get; set; }

        [JsonPropertyName("currentWeek")]
        public int CurrentWeek { get; set; }

        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonPropertyName("workouts")]
        public ICollection<WorkoutDto> Workouts { get; set; } = new List<WorkoutDto>();

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }

        [JsonPropertyName("client")]
        public ClientDto Client { get; set; } = null!;
    }

    public class CreateWeeklyProgramDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("durationInWeeks")]
        public int DurationInWeeks { get; set; }

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("workoutIds")]
        public ICollection<int>? WorkoutIds { get; set; }
    }

    public class UpdateWeeklyProgramDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("durationInWeeks")]
        public int? DurationInWeeks { get; set; }

        [JsonPropertyName("currentWeek")]
        public int? CurrentWeek { get; set; }

        [JsonPropertyName("workoutIds")]
        public ICollection<int>? WorkoutIds { get; set; }
    }
}