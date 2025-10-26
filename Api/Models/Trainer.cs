using Api.Models.ChildrenClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Trainer : PersonalInformation
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Bio cannot be longer than 1000 characters.")]
        public string? Bio { get; set; }
        
        //[Required] is unecessary for collections
        public ICollection<Client> Clients { get; set; } = new List<Client>();

        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

        public ICollection<TrainerSpecialization> Specializations { get; set; } = new List<TrainerSpecialization>();
        

        

    }
}