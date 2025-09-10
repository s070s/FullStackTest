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
        //Client Table
        public DbSet<Client> Clients => Set<Client>();
        //Equipment Table
        public DbSet<Equipment> Equipments => Set<Equipment>();
        //ExerciseDefinition Table
        public DbSet<ExerciseDefinition> ExerciseDefinitions => Set<ExerciseDefinition>();
        //Goal Table
        public DbSet<Goal> Goals => Set<Goal>();
        //Measurement Table
        public DbSet<Measurement> Measurements => Set<Measurement>();
        //MedicalHistory Table
        public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
        //Trainer Table
        public DbSet<Trainer> Trainers => Set<Trainer>();
        //User Table
        public DbSet<User> Users => Set<User>();
        //WeeklyProgram Table
        public DbSet<WeeklyProgram> WeeklyPrograms => Set<WeeklyProgram>();
        //Workout Table
        public DbSet<Workout> Workouts => Set<Workout>();
        //WorkoutExercise Table
        public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
        //WorkoutExerciseSet Table
        public DbSet<WorkoutExerciseSet> WorkoutExerciseSets => Set<WorkoutExerciseSet>();


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
