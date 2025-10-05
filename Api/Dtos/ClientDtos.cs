using Api.Models;
using Api.Models.Enums;
using System.Text.Json.Serialization;


namespace Api.Dtos
{
    public record ClientDto(
        int Id,
        int UserId,
        string? Bio,
        ClientExperience ExperienceLevel,
        MedicalHistoryDto? MedicalHistory,
        IntensityLevel PreferredIntensityLevel,
        string? FirstName,
        string? LastName,
        DateTime? DateOfBirth,
        string? PhoneNumber,
        string? Address,
        string? City,
        string? State,
        string? ZipCode,
        string? Country,
        double? Weight,
        double? Height,
        double? BMR,
        double? BMI,
        int? Age,
        ICollection<GoalDto> Goals,
        ICollection<WorkoutDto> Workouts,
        ICollection<TrainerDto> Trainers,
        ICollection<MeasurementDto> Measurements
    );

    public record CreateClientDto(
        int UserId,
        string? Bio = null,
        ClientExperience ExperienceLevel = ClientExperience.Beginner,
        MedicalHistoryDto? MedicalHistory = null,
        IntensityLevel PreferredIntensityLevel = IntensityLevel.Medium
    );

    //Todo convert all Records to classes
    public class UpdateClientProfileDto
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }
        [JsonPropertyName("bio")]
        public string? Bio { get; set; }
        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }
        [JsonPropertyName("height")]
        public double? Height { get; set; }
        [JsonPropertyName("weight")]
        public double? Weight { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("country")]
        public string? Country { get; set; }
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }
        [JsonPropertyName("state")]
        public string? State { get; set; }
        [JsonPropertyName("experienceLevel")]
        public ClientExperience? ExperienceLevel { get; set; }
    }
}