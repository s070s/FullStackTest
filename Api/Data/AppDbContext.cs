using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<User>();

            user.ToTable("Users");
            user.HasKey(u => u.Id);

            user.Property(u => u.Username).IsRequired().HasMaxLength(64);
            user.Property(u => u.Email).IsRequired().HasMaxLength(256);
            user.Property(u => u.CreatedUtc).IsRequired();

            user.HasIndex(u => u.Username).IsUnique();
            user.HasIndex(u => u.Email).IsUnique();
        }
    }
}
