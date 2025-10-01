using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class MedicalHistoryConfiguration : IEntityTypeConfiguration<MedicalHistory>
    {
        public void Configure(EntityTypeBuilder<MedicalHistory> builder)
        {
            builder.ToTable("MedicalHistories");

            builder.HasKey(mh => mh.Id);
            //One-To-One relationship with Client
            builder.HasOne(mh => mh.Client)
                   .WithOne(c => c.MedicalHistory)
                   .HasForeignKey<MedicalHistory>(mh => mh.ClientId)
                   .OnDelete(DeleteBehavior.Cascade);

            // NOTE: Ensure navigation properties in MedicalHistory and Client models are set up for this one-to-one relationship.

            // TIP: If you add new relationships or properties to MedicalHistory, update this configuration accordingly.

            // CAUTION: Cascade delete will remove the related medical history when a Client is deleted. Confirm this matches your business logic.

            // EF Core will enforce the one-to-one relationship and foreign key constraint based on this configuration.
        }
    }
}