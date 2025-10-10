using Api.Models.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Equipment : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<ExerciseDefinition> Exercises { get; set; } = new List<ExerciseDefinition>();
        
        
    }
}