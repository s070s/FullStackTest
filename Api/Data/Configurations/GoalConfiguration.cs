using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.ToTable("Goals");

            builder.HasKey(g => g.Id);

            builder.HasOne(g => g.Client)
                .WithMany(c => c.Goals)
                .HasForeignKey(g => g.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in Goal and Client models are set up for this one-to-many relationship.

            // TIP: If you add new relationships or properties to Goal, update this configuration accordingly.

            // CAUTION: Cascade delete will remove all related goals when a Client is deleted. Confirm this matches your business logic.

            // EF Core will handle foreign key constraints and relationships based on this configuration.
        }
    }
}