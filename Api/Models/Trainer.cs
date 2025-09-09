using Api.Models.ChildrenClasses;
using Api.Models.Enums;
namespace Api.Models
{
    public class Trainer : PersonalInformation
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Add trainer-specific properties
        public string? Bio { get; set; }
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public ICollection<TrainerSpecialization> Specializations { get; set; } = new List<TrainerSpecialization>();
        //Workout properties
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    }
}