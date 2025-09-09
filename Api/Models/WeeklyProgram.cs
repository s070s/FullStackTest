namespace Api.Models
{
    public class WeeklyProgram : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DurationInWeeks { get; set; }
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
        // Foreign key to Client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
    }
}