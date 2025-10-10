using System.Text.Json.Serialization;
using Api.Models.Enums;

namespace Api.Dtos
{
    public class MeasurementDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("unit")]
        public GoalUnit Unit { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("intensity")]
        public IntensityLevel Intensity { get; set; }

        [JsonPropertyName("isPersonalBest")]
        public bool IsPersonalBest { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }

        [JsonPropertyName("client")]
        public ClientDto Client { get; set; } = null!;
    }

    public class CreateMeasurementDto
    {
        [JsonPropertyName("unit")]
        public GoalUnit Unit { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("intensity")]
        public IntensityLevel Intensity { get; set; }

        [JsonPropertyName("isPersonalBest")]
        public bool IsPersonalBest { get; set; } = false;

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; } = 0;
    }

    public class UpdateMeasurementDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("unit")]
        public GoalUnit? Unit { get; set; }

        [JsonPropertyName("value")]
        public double? Value { get; set; }

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("intensity")]
        public IntensityLevel? Intensity { get; set; }

        [JsonPropertyName("isPersonalBest")]
        public bool? IsPersonalBest { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }
}