using Api.Models.Enums;
using System.Text.Json.Serialization;


namespace Api.Dtos
{
    public class ClientDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("bio")]
        public string? Bio { get; set; }
        [JsonPropertyName("experienceLevel")]
        public ClientExperience ExperienceLevel { get; set; }
        [JsonPropertyName("medicalHistory")]
        public MedicalHistoryDto? MedicalHistory { get; set; }
        [JsonPropertyName("preferredIntensityLevel")]
        public IntensityLevel PreferredIntensityLevel { get; set; }
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }
        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("state")]
        public string? State { get; set; }
        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }
        [JsonPropertyName("country")]
        public string? Country { get; set; }
        [JsonPropertyName("weight")]
        public double? Weight { get; set; }
        [JsonPropertyName("height")]
        public double? Height { get; set; }
        [JsonPropertyName("bmr")]
        public double? BMR { get; set; }
        [JsonPropertyName("bmi")]
        public double? BMI { get; set; }
        [JsonPropertyName("age")]
        public int? Age { get; set; }
        [JsonPropertyName("goals")]
        public ICollection<GoalDto> Goals { get; set; } = new List<GoalDto>();
        [JsonPropertyName("workouts")]
        public ICollection<WorkoutDto> Workouts { get; set; } = new List<WorkoutDto>();
        [JsonPropertyName("trainers")]
        public ICollection<TrainerDto> Trainers { get; set; } = new List<TrainerDto>();
        [JsonPropertyName("measurements")]
        public ICollection<MeasurementDto> Measurements { get; set; } = new List<MeasurementDto>();
        [JsonPropertyName("profilePhotoUrl")]
        public string? ProfilePhotoUrl { get; set; }
    }

public class CreateClientDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("bio")]
        public string? Bio { get; set; }
        [JsonPropertyName("experienceLevel")]
        public ClientExperience ExperienceLevel { get; set; } = ClientExperience.Beginner;
        [JsonPropertyName("medicalHistory")]
        public MedicalHistoryDto? MedicalHistory { get; set; }
        [JsonPropertyName("preferredIntensityLevel")]
        public IntensityLevel PreferredIntensityLevel { get; set; } = IntensityLevel.Medium;
    }

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