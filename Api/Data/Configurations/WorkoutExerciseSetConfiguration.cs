using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class WorkoutExerciseSetConfiguration : IEntityTypeConfiguration<WorkoutExerciseSet>
    {
        public void Configure(EntityTypeBuilder<WorkoutExerciseSet> builder)
        {
            builder.ToTable("WorkoutExerciseSets");

            builder.HasKey(wes => wes.Id);

            builder.HasOne(wes => wes.WorkoutExercise)
                .WithMany(we => we.Sets)
                .HasForeignKey(wes => wes.WorkoutExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in WorkoutExerciseSet and WorkoutExercise models are set up for this one-to-many relationship.

            // TIP: If you add new relationships or properties to WorkoutExerciseSet, update this configuration accordingly.

            // CAUTION: Cascade delete will remove all related sets when a WorkoutExercise is deleted. Confirm this matches your business logic.

            // EF Core will enforce one-to-many relationships and foreign key constraints based on this configuration.
        }
    }
}