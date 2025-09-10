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
        }
    }
}