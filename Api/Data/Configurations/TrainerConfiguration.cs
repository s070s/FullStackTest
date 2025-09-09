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
            builder.HasOne(t => t.User)
                .WithOne(u => u.TrainerProfile)
                .HasForeignKey<Trainer>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}