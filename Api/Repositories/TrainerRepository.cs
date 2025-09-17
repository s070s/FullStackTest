using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public interface ITrainerRepository
    {
        Task AddTrainerAsync(Trainer trainer);
        Task<Trainer?> GetTrainerByUserIdAsync(int userId);
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
    }
}