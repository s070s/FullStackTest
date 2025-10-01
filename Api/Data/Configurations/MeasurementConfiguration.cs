using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class MeasurementConfiguration : IEntityTypeConfiguration<Measurement>
    {
        public void Configure(EntityTypeBuilder<Measurement> builder)
        {
            builder.ToTable("Measurements");

            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Client)
                .WithMany(c => c.Measurements)
                .HasForeignKey(m => m.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in Measurement and Client models are set up for this one-to-many relationship.

            // TIP: If you add new relationships or properties to Measurement, update this configuration accordingly.

            // CAUTION: Cascade delete will remove all related measurements when a Client is deleted. Confirm this matches your business logic.

            // EF Core will handle foreign key constraints and relationships based on this configuration.
        }
    }
}