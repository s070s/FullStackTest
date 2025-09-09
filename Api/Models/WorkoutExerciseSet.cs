namespace Api.Models
{
    public class WorkoutExerciseSet : BaseEntity
    {
        public int WorkoutExerciseId { get; set; }
        public WorkoutExercise WorkoutExercise { get; set; } = null!;
        public int SetNumber { get; set; }
        public int Repetitions { get; set; }
        public double Weight { get; set; } // in kg
        public IntensityLevel OverallIntensityLevel { get; set; }
        public int DurationInSeconds { get; set; } // Duration for timed exercises
        public string? Notes { get; set; } // Any additional notes or instructions
    }
    
}