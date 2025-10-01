using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models;

namespace Api.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        //Configure User entity and its relationships
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(16);

            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();

            // Enforce relationships with Client, Trainer
            builder.HasOne(u => u.TrainerProfile)
                .WithOne(t => t.User)
                .HasForeignKey<Trainer>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.ClientProfile)
                .WithOne(c => c.User)
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in User, Trainer, and Client models are set up for these one-to-one relationships.

            // TIP: If you add new relationships or properties to User, update this configuration accordingly.

            // CAUTION: Cascade deletes will remove related Trainer or Client entities when a User is deleted. Confirm this matches your business logic.

            // EF Core will enforce unique constraints and relationships based on this configuration.
        }
    }
}