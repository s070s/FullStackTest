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
        }
    }
}