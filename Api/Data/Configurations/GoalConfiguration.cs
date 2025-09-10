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
        }
    }
}