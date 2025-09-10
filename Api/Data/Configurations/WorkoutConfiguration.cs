using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models;

namespace Api.Data.Configurations
{
    public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
    {
        //Configure Workout entity and its relationships
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.ToTable("Workouts");
            builder.HasKey(w => w.Id);

            // Many-to-many: Workout <-> Client
            builder
                .HasMany(w => w.Clients)
                .WithMany(c => c.Workouts)
                .UsingEntity(j => j.ToTable("ClientWorkouts"));

            // One-to-many: Trainer -> Workouts
            builder
                .HasOne(w => w.Trainer)
                .WithMany(t => t.Workouts)
                .HasForeignKey(w => w.TrainerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Optional: Workout -> WeeklyProgram (many-to-one)
            builder
                .HasOne(w => w.WeeklyProgram)
                .WithMany(wp => wp.Workouts)
                .HasForeignKey(w => w.WeeklyProgramId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}