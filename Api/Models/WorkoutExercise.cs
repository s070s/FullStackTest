namespace Api.Models
{
    public class WorkoutExercise:BaseEntity
    {
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;
        public int ExerciseDefinitionId { get; set; }
        public ExerciseDefinition ExerciseDefinition { get; set; } = null!;
        public ICollection<WorkoutExerciseSet> Sets { get; set; } = new List<WorkoutExerciseSet>();
        public int DurationInSeconds { get; set; } // Duration for timed exercises
        public string? Notes { get; set; } // Any additional notes or instructions

    }
}