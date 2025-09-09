using Api.Models.BaseClasses;
using Api.Models.Enums;
namespace Api.Models
{
    public class Goal : BaseEntity
    {
        public GoalType GoalType { get; set; }
        public string Description { get; set; } = null!;
        public DateTime TargetDate { get; set; }
        public bool IsAchieved { get; set; }

        // Example: 5 (for 5 kg, 5 lbs, etc.)

        public int? GoalQuantity { get; set; }
        public GoalUnit? GoalUnit { get; set; }

        // Foreign key to Client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;


    }
}