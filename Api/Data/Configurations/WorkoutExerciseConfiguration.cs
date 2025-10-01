using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
    {
        public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
        {
            builder.ToTable("WorkoutExercises");

            builder.HasKey(we => we.Id);

            builder.HasOne(we => we.Workout)
                .WithMany(w => w.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            //One Exercise Definition to Many Workout Exercises
            builder.HasOne(we => we.ExerciseDefinition)
                .WithMany(ed => ed.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(we => we.Sets)
                .WithOne(s => s.WorkoutExercise)
                .HasForeignKey(s => s.WorkoutExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in WorkoutExercise, Workout, ExerciseDefinition, and Set models are set up for these relationships.

            // TIP: If you add new relationships or properties to WorkoutExercise, update this configuration accordingly.

            // CAUTION: Cascade deletes will remove related WorkoutExercises or Sets when a Workout or WorkoutExercise is deleted. DeleteBehavior.Restrict prevents deleting ExerciseDefinition if referenced.

            // EF Core will enforce one-to-many and many-to-one relationships and foreign key constraints based on this configuration.
        }
    }
}