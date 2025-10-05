using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

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
        
        public EquipmentDto ToEquipmentDto()
        {
            return new EquipmentDto(
                Id,
                Name,
                Description,
                Exercises.Select(e => e.ToExerciseDefinitionDto()).ToList()
            );
        }

        public CreateEquipmentDto ToCreateEquipmentDto()
        {
            return new CreateEquipmentDto(
                Name,
                Description,
                Exercises.Select(e => e.Id).ToList()
            );
        }

        public UpdateEquipmentDto ToUpdateEquipmentDto()
        {
            return new UpdateEquipmentDto(
                Id,
                Name,
                Description,
                Exercises.Select(e => e.Id).ToList()
            );
        }
    }
}