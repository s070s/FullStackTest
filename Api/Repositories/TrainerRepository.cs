using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;

namespace Api.Repositories
{
    // Interface for Trainer repository, defines contract for Trainer data operations
    public interface ITrainerRepository
    {
        // Adds a new Trainer to the database
        Task AddTrainerAsync(Trainer trainer);

        // Retrieves a Trainer by the associated UserId, or null if not found
        Task<Trainer?> GetTrainerByUserIdAsync(int userId);

        // Updates a Trainer's profile using the provided DTO, returns updated Trainer or null if not found
        Task<Trainer?> UpdateTrainerProfileAsync(int userId, UpdateTrainerProfileDto updatedTrainer);
    }

    // Concrete implementation of ITrainerRepository using Entity Framework Core
    public class TrainerRepository : ITrainerRepository
    {
        private readonly AppDbContext _db;

        // Injects the application's DbContext
        public TrainerRepository(AppDbContext db)
        {
            _db = db;
        }

        // Adds a new Trainer entity and saves changes asynchronously
        public async Task AddTrainerAsync(Trainer trainer)
        {
            _db.Trainers.Add(trainer);
            await _db.SaveChangesAsync();
        }

        // Retrieves a Trainer entity by UserId, returns null if not found
        public async Task<Trainer?> GetTrainerByUserIdAsync(int userId)
        {
            return await _db.Trainers.SingleOrDefaultAsync(t => t.UserId == userId);
        }

        // Updates an existing Trainer's profile fields and saves changes
        // Returns the updated Trainer or null if not found
        public async Task<Trainer?> UpdateTrainerProfileAsync(int userId, UpdateTrainerProfileDto updatedTrainer)
        {
            var existingTrainer = await _db.Trainers.SingleOrDefaultAsync(t => t.UserId == userId);
            if (existingTrainer == null) return null;

            // Update fields from DTO
            existingTrainer.FirstName = updatedTrainer.FirstName;
            existingTrainer.LastName = updatedTrainer.LastName;
            existingTrainer.Bio = updatedTrainer.Bio;
            existingTrainer.DateOfBirth = updatedTrainer.DateOfBirth;
            existingTrainer.Height = updatedTrainer.Height;
            existingTrainer.Weight = updatedTrainer.Weight;
            existingTrainer.PhoneNumber = updatedTrainer.PhoneNumber;
            existingTrainer.Country = updatedTrainer.Country;
            existingTrainer.City = updatedTrainer.City;
            existingTrainer.Address = updatedTrainer.Address;
            existingTrainer.ZipCode = updatedTrainer.ZipCode;
            existingTrainer.State = updatedTrainer.State;

            await _db.SaveChangesAsync();
            return existingTrainer;
        }
    }
}