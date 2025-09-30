using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;

namespace Api.Repositories
{
    public interface ITrainerRepository
    {
        Task AddTrainerAsync(Trainer trainer);
        Task<Trainer?> GetTrainerByUserIdAsync(int userId);
        Task<Trainer?> UpdateTrainerProfileAsync(int userId, UpdateTrainerProfileDto updatedTrainer);
    }

    public class TrainerRepository : ITrainerRepository
    {
        private readonly AppDbContext _db;
        public TrainerRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddTrainerAsync(Trainer trainer)
        {
            _db.Trainers.Add(trainer);
            await _db.SaveChangesAsync();
        }

        public async Task<Trainer?> GetTrainerByUserIdAsync(int userId)
        {
            return await _db.Trainers.SingleOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<Trainer?> UpdateTrainerProfileAsync(int userId, UpdateTrainerProfileDto updatedTrainer)
        {
            var existingTrainer = await _db.Trainers.SingleOrDefaultAsync(t => t.UserId == userId);
            if (existingTrainer == null) return null;

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