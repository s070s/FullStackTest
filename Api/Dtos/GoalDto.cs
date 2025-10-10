using System.Text.Json.Serialization;
using Api.Models.Enums;

namespace Api.Dtos
{
    public class GoalDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("goalType")]
        public GoalType GoalType { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("targetDate")]
        public DateTime TargetDate { get; set; }

        [JsonPropertyName("status")]
        public GoalStatus Status { get; set; }

        [JsonPropertyName("goalQuantity")]
        public int? GoalQuantity { get; set; }

        [JsonPropertyName("goalUnit")]
        public GoalUnit? GoalUnit { get; set; }

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }

        [JsonPropertyName("client")]
        public ClientDto Client { get; set; } = null!;
    }

    public class CreateGoalDto
    {
        [JsonPropertyName("goalType")]
        public GoalType GoalType { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("targetDate")]
        public DateTime TargetDate { get; set; }

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }

        [JsonPropertyName("status")]
        public GoalStatus Status { get; set; }

        [JsonPropertyName("goalQuantity")]
        public int? GoalQuantity { get; set; }

        [JsonPropertyName("goalUnit")]
        public GoalUnit? GoalUnit { get; set; }
    }

    public class UpdateGoalDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("goalType")]
        public GoalType? GoalType { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("targetDate")]
        public DateTime? TargetDate { get; set; }

        [JsonPropertyName("status")]
        public GoalStatus? Status { get; set; }

        [JsonPropertyName("goalQuantity")]
        public int? GoalQuantity { get; set; }

        [JsonPropertyName("goalUnit")]
        public GoalUnit? GoalUnit { get; set; }
    }
}