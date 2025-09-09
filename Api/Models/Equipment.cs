using Api.Models.BaseClasses;

namespace Api.Models
{
    public class Equipment : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<ExerciseDefinition> Exercises { get; set; } = new List<ExerciseDefinition>();
    }
}