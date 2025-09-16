using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

namespace Api.Models
{
    public class Goal : BaseEntity
    {
        [Required]
        public GoalType GoalType { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime TargetDate { get; set; }

        [Required]
        public GoalStatus Status { get; set; }

        // Example: 5 (for 5 kg, 5 lbs, etc.)
        [Range(0, 1000)]
        public int? GoalQuantity { get; set; }

        public GoalUnit? GoalUnit { get; set; }

        // Foreign key to Client
        [Required]
        public int ClientId { get; set; }

        [Required]
        public Client Client { get; set; } = null!;


        public GoalDto ToGoalDto()
        {
            return new GoalDto(
                Id,
                GoalType,
                Description,
                TargetDate,
                Status,
                GoalQuantity,
                GoalUnit,
                ClientId,
                Client.ToClientDto()
            );
        }

        public CreateGoalDto ToCreateGoalDto()
        {
            return new CreateGoalDto(
                GoalType,
                Description,
                TargetDate,
                ClientId,
                Status,
                GoalQuantity,
                GoalUnit
            );
        }

        public UpdateGoalDto ToUpdateGoalDto()
        {
            return new UpdateGoalDto(
                Id,
                GoalType,
                Description,
                TargetDate,
                Status,
                GoalQuantity,
                GoalUnit
            );
        }


    }
}