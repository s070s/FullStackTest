using Microsoft.EntityFrameworkCore;
using Api.Models;
using Api.Models.BaseClasses;
using Api.Services;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IDbContextLifecycleService _DbContextLifecycleService;
        public AppDbContext(DbContextOptions<AppDbContext> options, IDbContextLifecycleService dbContextLifecycleService) : base(options)
        {
            _DbContextLifecycleService = dbContextLifecycleService;
        }
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
            // Apply all configurations from Data/Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            _DbContextLifecycleService.UpdateTimestamps(ChangeTracker);
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _DbContextLifecycleService.UpdateTimestamps(ChangeTracker);
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
