using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //User Table
        public DbSet<User> Users => Set<User>();
        //Trainer Table
        public DbSet<Trainer> Trainers => Set<Trainer>();
        //Client Table
        public DbSet<Client> Clients => Set<Client>();
        //Workout Table
        public DbSet<Workout> Workouts => Set<Workout>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure User entity and its relationships
            var user = modelBuilder.Entity<User>();

            user.ToTable("Users");
            user.HasKey(u => u.Id);

            user.Property(u => u.Username).IsRequired().HasMaxLength(64);
            user.Property(u => u.Email).IsRequired().HasMaxLength(256);
            user.Property(u => u.CreatedUtc).IsRequired();
            user.Property(u => u.PasswordHash).IsRequired();
            user.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(16);

            user.HasIndex(u => u.Username).IsUnique();
            user.HasIndex(u => u.Email).IsUnique();

            //Configure Trainer entity and its relationships
            modelBuilder.Entity<Trainer>()
            .HasOne(t => t.User)
            .WithOne(u => u.TrainerProfile)
            .HasForeignKey<Trainer>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            //Configure Client entity and its relationships
            modelBuilder.Entity<Client>()
            .HasOne(c => c.User)
            .WithOne(u => u.ClientProfile)
            .HasForeignKey<Client>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            //Configure Workout and Client entity relationships
            modelBuilder.Entity<Workout>()
            .HasOne(w => w.Client)
            .WithMany(c => c.Workouts)
            .HasForeignKey(w => w.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

            //Configure Workout and Trainer entity relationships
            modelBuilder.Entity<Workout>()
            .HasOne(w => w.Trainer)
            .WithMany(t => t.Workouts)
            .HasForeignKey(w => w.TrainerId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
