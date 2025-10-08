using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

namespace Api.Models
{
    public class Workout : BaseEntity
    {
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();

        public int? TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        public int? WeeklyProgramId { get; set; }
        public WeeklyProgram? WeeklyProgram { get; set; }

        [Required]
        public DateTime ScheduledDateTime { get; set; }

        [Required]
        [StringLength(50)]
        public string? Type { get; set; }

        [Range(1, 300)]
        public int DurationInMinutes { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
        
        

    }
}