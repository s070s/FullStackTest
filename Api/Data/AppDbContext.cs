using Microsoft.EntityFrameworkCore;
using Api.Models;
using Api.Models.BaseClasses;
using Api.Services;

namespace Api.Data
{
    // Main EF Core DbContext for the application.
    // Handles all database tables and entity configurations.
    public class AppDbContext : DbContext
    {
        // Service for handling entity lifecycle events (e.g., timestamps).
        private readonly IDbContextLifecycleService _DbContextLifecycleService;

        // Constructor: injects DbContext options and lifecycle service.
        public AppDbContext(DbContextOptions<AppDbContext> options, IDbContextLifecycleService dbContextLifecycleService) : base(options)
        {
            _DbContextLifecycleService = dbContextLifecycleService;
        }

        // DbSets represent tables in the database.
        // Each property exposes a table for CRUD operations.

        // Client Table
        public DbSet<Client> Clients => Set<Client>();
        // Equipment Table
        public DbSet<Equipment> Equipments => Set<Equipment>();
        // ExerciseDefinition Table
        public DbSet<ExerciseDefinition> ExerciseDefinitions => Set<ExerciseDefinition>();
        // Goal Table
        public DbSet<Goal> Goals => Set<Goal>();
        // Measurement Table
        public DbSet<Measurement> Measurements => Set<Measurement>();
        // MedicalHistory Table
        public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
        // Trainer Table
        public DbSet<Trainer> Trainers => Set<Trainer>();
        // User Table
        public DbSet<User> Users => Set<User>();
    // RefreshToken Table
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        // WeeklyProgram Table
        public DbSet<WeeklyProgram> WeeklyPrograms => Set<WeeklyProgram>();
        // Workout Table
        public DbSet<Workout> Workouts => Set<Workout>();
        // WorkoutExercise Table
        public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
        // WorkoutExerciseSet Table
        public DbSet<WorkoutExerciseSet> WorkoutExerciseSets => Set<WorkoutExerciseSet>();

        // Configures entity mappings and relationships.
        // Applies all IEntityTypeConfiguration<T> found in the assembly.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations from Data/Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        // Overrides SaveChanges to update timestamps before saving.
        public override int SaveChanges()
        {
            _DbContextLifecycleService.UpdateTimestamps(ChangeTracker);
            return base.SaveChanges();
        }

        // Async version of SaveChanges with timestamp update.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _DbContextLifecycleService.UpdateTimestamps(ChangeTracker);
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
