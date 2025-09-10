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
        }
    }
}