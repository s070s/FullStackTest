using Api.Models.BaseClasses;

namespace Api.Models
{
	public class ExerciseDefinition:BaseEntity
	{
		public string? Name { get; set; }
		//e.g. A description of how to perform the exercise along any cues/tips
		public string? Description { get; set; }
		public string? VideoUrl { get; set; }
		public int CaloriesBurnedPerHour { get; set; }
		//e.g. Dumbbells, Barbell, Kettlebell, Machine, Bodyweight
		public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
		//more than one muscle group used=>compound
		public bool IsCompoundExercise { get; set; }

		//e.g. Chest, Back, Legs, Arms, Shoulders, Core
		public ICollection<MuscleGroup> PrimaryMuscleGroups { get; set; } = new List<MuscleGroup>();
		public ICollection<MuscleGroup> SecondaryMuscleGroups { get; set; } = new List<MuscleGroup>();
		//eg. Beginner, Intermediate, Advanced
		public ExperienceLevel? ExperienceLevel { get; set; }
		//e.g. Strength, Cardio, Flexibility, Balance
		public string? Category { get; set; }
	}
}