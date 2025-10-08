using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

namespace Api.Models
{
    public class Measurement : BaseEntity
    {


        [Required]
        public GoalUnit Unit { get; set; }

        [Required]
        [Range(0, 1000.0, ErrorMessage = "Value must be non-negative.")]
        public double Value { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public IntensityLevel Intensity { get; set; }

        public bool IsPersonalBest { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public Client Client { get; set; } = null!;

        
    }
}