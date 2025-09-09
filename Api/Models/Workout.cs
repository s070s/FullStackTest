namespace Api.Models
{
    public class Workout:BaseEntity
    {
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;

        // Add workout-specific properties
        public DateTime Date { get; set; }
        public string? Type { get; set; }
        public int DurationInMinutes { get; set; }
        public string? Notes { get; set; }
    }
}