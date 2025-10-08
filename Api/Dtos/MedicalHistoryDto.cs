using System.Text.Json.Serialization;
using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public class MedicalHistoryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("conditions")]
        public ICollection<MedicalCondition> Conditions { get; set; } = new List<MedicalCondition>();

        [JsonPropertyName("medicationTypes")]
        public ICollection<MedicationType> MedicationTypes { get; set; } = new List<MedicationType>();

        [JsonPropertyName("surgeries")]
        public ICollection<SurgeryType> Surgeries { get; set; } = new List<SurgeryType>();

        [JsonPropertyName("recommendedIntensityLevel")]
        public IntensityLevel? RecommendedIntensityLevel { get; set; }

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }

        [JsonPropertyName("client")]
        public ClientDto Client { get; set; } = null!;
    }

    public class CreateMedicalHistoryDto
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("conditions")]
        public ICollection<MedicalCondition> Conditions { get; set; } = new List<MedicalCondition>();

        [JsonPropertyName("medicationTypes")]
        public ICollection<MedicationType> MedicationTypes { get; set; } = new List<MedicationType>();

        [JsonPropertyName("surgeries")]
        public ICollection<SurgeryType> Surgeries { get; set; } = new List<SurgeryType>();

        [JsonPropertyName("recommendedIntensityLevel")]
        public IntensityLevel? RecommendedIntensityLevel { get; set; }

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }
    }

    public class UpdateMedicalHistoryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("conditions")]
        public ICollection<MedicalCondition>? Conditions { get; set; }

        [JsonPropertyName("medicationTypes")]
        public ICollection<MedicationType>? MedicationTypes { get; set; }

        [JsonPropertyName("surgeries")]
        public ICollection<SurgeryType>? Surgeries { get; set; }

        [JsonPropertyName("recommendedIntensityLevel")]
        public IntensityLevel? RecommendedIntensityLevel { get; set; }
    }
}