using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class WeeklyProgramConfiguration : IEntityTypeConfiguration<WeeklyProgram>
    {
        public void Configure(EntityTypeBuilder<WeeklyProgram> builder)
        {
            builder.ToTable("WeeklyPrograms");

            builder.HasKey(wp => wp.Id);

            builder.HasOne(wp => wp.Client)
                .WithOne(c => c.CurrentWeeklyProgram)
                .HasForeignKey<WeeklyProgram>(wp => wp.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(wp => wp.Workouts)
                .WithOne(w => w.WeeklyProgram)
                .HasForeignKey(w => w.WeeklyProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in WeeklyProgram, Client, and Workout models are set up for these relationships.

            // TIP: If you add new relationships or properties to WeeklyProgram, update this configuration accordingly.

            // CAUTION: Cascade deletes will remove related WeeklyProgram or Workouts when a Client or WeeklyProgram is deleted. Confirm this matches your business logic.

            // EF Core will enforce one-to-one and one-to-many relationships and foreign key constraints based on this configuration.
        }
    }
}