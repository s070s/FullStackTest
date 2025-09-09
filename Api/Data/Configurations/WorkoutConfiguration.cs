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
            builder.HasOne(w => w.Client)
                .WithMany(c => c.Workouts)
                .HasForeignKey(w => w.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.Trainer)
                .WithMany(t => t.Workouts)
                .HasForeignKey(w => w.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}