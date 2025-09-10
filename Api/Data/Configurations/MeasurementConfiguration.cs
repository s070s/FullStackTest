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
        }
    }
}