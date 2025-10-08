using System.Text.Json.Serialization;
using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public class EquipmentDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("exercises")]
        public ICollection<ExerciseDefinitionDto> Exercises { get; set; } = new List<ExerciseDefinitionDto>();
    }

    public class CreateEquipmentDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("exerciseIds")]
        public ICollection<int>? ExerciseIds { get; set; }
    }

    public class UpdateEquipmentDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("exerciseIds")]
        public ICollection<int>? ExerciseIds { get; set; }
    }
}