using Api.Models.ChildrenClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

namespace Api.Models
{
    public class Client : PersonalInformation
    {

        // Add client-specific properties
        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string? Bio { get; set; }

        [Required]
        public ClientExperience ExperienceLevel { get; set; }

        public MedicalHistory? MedicalHistory { get; set; }

        [Required]
        public IntensityLevel PreferredIntensityLevel { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; } = null!;

        public WeeklyProgram? CurrentWeeklyProgram { get; set; }

        public ICollection<Goal> Goals { get; set; } = new List<Goal>();

        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();

        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

        public ClientDto ToClientDto()
        {
            return new ClientDto(
                Id,
                UserId,
                Bio,
                ExperienceLevel,
                MedicalHistory,
                PreferredIntensityLevel,
                Goals,
                Workouts,
                Trainers,
                Measurements
            );
        }

        public CreateClientDto ToCreateClientDto()
        {
            return new CreateClientDto(
                UserId,
                Bio,
                ExperienceLevel,
                MedicalHistory,
                PreferredIntensityLevel
            );
        }

        public UpdateClientDto ToUpdateClientDto()
        {
            return new UpdateClientDto(
                Id,
                Bio,
                ExperienceLevel,
                MedicalHistory,
                PreferredIntensityLevel
            );
        }

    }
}