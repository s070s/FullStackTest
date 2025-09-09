namespace Api.Models
{
    public class Trainer: BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Add trainer-specific properties
        public string? Bio { get; set; }
        public string? Specialization { get; set; }

        //Workout properties
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    }
}