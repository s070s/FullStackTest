namespace Api.Models
{
    public class Client
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Add client-specific properties
        public string? Bio { get; set; }
        public string? Goals { get; set; }
        public string? Experience { get; set; }
        public string? MedicalInformation { get; set; }

        //Workout properties
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    }
}