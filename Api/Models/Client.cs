using Api.Models.ChildrenClasses;
using Api.Models.Enums;
namespace Api.Models
{
    public class Client : PersonalInformation
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Add client-specific properties
        public string? Bio { get; set; }
        public Goal? CurrentGoal { get; set; }
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public ICollection<Goal> CompletedGoals { get; set; } = new List<Goal>();
        public ClientExperience ExperienceLevel { get; set; }
        public MedicalHistory? MedicalHistory { get; set; }
        
        public IntensityLevel PreferredIntensityLevel { get; set; }

        //Workout properties
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    }
}