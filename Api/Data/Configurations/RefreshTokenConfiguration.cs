using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(rt => rt.TokenHash)
                .IsUnique();

            builder.Property(rt => rt.ReplacedByTokenHash)
                .HasMaxLength(128);

            builder.Property(rt => rt.CreatedByIp)
                .HasMaxLength(45);

            builder.Property(rt => rt.RevokedByIp)
                .HasMaxLength(45);

            builder.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
