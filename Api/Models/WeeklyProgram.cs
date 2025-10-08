using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

namespace Api.Models
{
    public class WeeklyProgram : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(1, 52)]
        public int DurationInWeeks { get; set; }

        [Range(1, 52)]
        public int CurrentWeek { get; set; } = 1;

        public bool IsCompleted => CurrentWeek > DurationInWeeks;

        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

        [Required]
        public int ClientId { get; set; }

        [Required]
        public Client Client { get; set; } = null!;


        
    }
}