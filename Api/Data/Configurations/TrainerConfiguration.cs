using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models;

namespace Api.Data.Configurations
{
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        //Configure Trainer entity and its relationships
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {

            builder.ToTable("Trainers");
            builder.HasKey(t => t.Id);
            
            // One-to-one relationship with User
            builder.HasOne(t => t.User)
                .WithOne(u => u.TrainerProfile)
                .HasForeignKey<Trainer>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many relationship with Clients
            builder.HasMany(t => t.Clients)
                .WithMany(c => c.Trainers)
                .UsingEntity(j => j.ToTable("ClientTrainers"));

            // One-to-many relationship with Workouts
            builder.HasMany(t => t.Workouts)
                .WithOne(w => w.Trainer)
                .HasForeignKey(w => w.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in Trainer, User, Client, and Workout models are set up for these relationships.

            // TIP: If you add new relationships or properties to Trainer, update this configuration accordingly.

            // EF Core will automatically create join tables for many-to-many relationships if not explicitly defined.

            // CAUTION: Cascade deletes will remove related entities when a Trainer is deleted. Review if this is the intended behavior for your use case.
        }
    }
}