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

            // NOTE: Ensure navigation properties in Workout, Client, Trainer, and WeeklyProgram models are set up for these relationships.

            // TIP: If you add new relationships or properties to Workout, update this configuration accordingly.

            // CAUTION: DeleteBehavior.SetNull will set foreign keys to null when related Trainer or WeeklyProgram is deleted. Confirm this matches your business logic.

            // EF Core will automatically create join tables for many-to-many relationships if not explicitly defined.
        }
    }
}