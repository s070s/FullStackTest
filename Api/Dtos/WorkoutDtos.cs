using System.Text.Json.Serialization;
using Api.Models.Enums;
using Api.Models;

namespace Api.Dtos
{
    public class WorkoutDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("clients")]
        public ICollection<ClientDto> Clients { get; set; } = new List<ClientDto>();

        [JsonPropertyName("trainerId")]
        public int? TrainerId { get; set; }

        [JsonPropertyName("trainer")]
        public TrainerDto? Trainer { get; set; }

        [JsonPropertyName("weeklyProgramId")]
        public int? WeeklyProgramId { get; set; }

        [JsonPropertyName("weeklyProgram")]
        public WeeklyProgramDto? WeeklyProgram { get; set; }

        [JsonPropertyName("scheduledDateTime")]
        public DateTime ScheduledDateTime { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("durationInMinutes")]
        public int DurationInMinutes { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    public class CreateWorkoutDto
    {
        [JsonPropertyName("clientIds")]
        public ICollection<int> ClientIds { get; set; } = new List<int>();

        [JsonPropertyName("trainerId")]
        public int? TrainerId { get; set; }

        [JsonPropertyName("weeklyProgramId")]
        public int? WeeklyProgramId { get; set; }

        [JsonPropertyName("scheduledDateTime")]
        public DateTime ScheduledDateTime { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("durationInMinutes")]
        public int DurationInMinutes { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    public class UpdateWorkoutDto
    {
        [JsonPropertyName("clientIds")]
        public ICollection<int>? ClientIds { get; set; }

        [JsonPropertyName("trainerId")]
        public int? TrainerId { get; set; }

        [JsonPropertyName("weeklyProgramId")]
        public int? WeeklyProgramId { get; set; }

        [JsonPropertyName("scheduledDateTime")]
        public DateTime? ScheduledDateTime { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("durationInMinutes")]
        public int? DurationInMinutes { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }
}