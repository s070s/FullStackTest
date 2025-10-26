using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models;

namespace Api.Data.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {

            builder.ToTable("Clients");
            builder.HasKey(c => c.Id);

            //One-to-One relationship between Client and User
            builder.HasOne(c => c.User)
                .WithOne(u => u.ClientProfile)
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure 1:1 by making UserId unique on Clients
            builder.HasIndex(c => c.UserId).IsUnique();

            //One-to-Many relationship between Client and Goals
            builder.HasMany(c => c.Goals)
                .WithOne(g => g.Client)
                .HasForeignKey(g => g.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            //Many-to-Many relationship between Client and Workouts
            builder.HasMany(c => c.Workouts)
                .WithMany(w => w.Clients)
                .UsingEntity(j => j.ToTable("ClientWorkouts"));
            //Many-to-Many relationship between Client and Trainers
            builder.HasMany(c => c.Trainers)
                .WithMany(t => t.Clients)
                .UsingEntity(j => j.ToTable("ClientTrainers"));
            //One-to-many relationship between Client and Measurements
            builder.HasMany(c => c.Measurements)
                .WithOne(m => m.Client)
                .HasForeignKey(m => m.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            //One-To-One relationship between Client and MedicalHistory
            builder.HasOne(c => c.MedicalHistory)
                .WithOne(m => m.Client)
                .HasForeignKey<MedicalHistory>(m => m.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in Client, User, Goal, Workout, Trainer, Measurement, and MedicalHistory models are properly set up for these relationships.

            // TIP: When adding new relationships or properties to the Client entity, update this configuration to maintain consistency.

            // EF Core will automatically create join tables for many-to-many relationships if not explicitly defined.

            // CAUTION: Cascade deletes will remove related entities when a Client is deleted. Review if this is the intended behavior for your use case.
        }
    }
}