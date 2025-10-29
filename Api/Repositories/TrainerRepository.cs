using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;

namespace Api.Repositories
{



    #region Interface
    /// <summary>
    /// Repository for managing Trainer entities and related operations.
    /// </summary>
    public interface ITrainerRepository
    {
        /// <summary>
        /// Adds a new Trainer to the database.
        /// </summary>
        Task AddTrainerAsync(Trainer trainer);

        /// <summary>
        /// Retrieves a Trainer by the associated UserId, or null if not found.
        /// </summary>
        Task<Trainer?> GetTrainerByUserIdAsync(int userId);

        /// <summary>
        /// Updates a Trainer's profile using the provided DTO, returns updated Trainer or null if not found.
        /// </summary>
        Task<Trainer?> UpdateTrainerProfileAsync(int userId, UpdateTrainerProfileDto updatedTrainer);

        /// <summary>
        /// Retrieves a paginated list of Trainers with optional sorting, returns the list and total count.
        /// </summary>
        Task<(IEnumerable<Trainer> trainers, int total)> GetTrainersPagedAsync(IPaginationService paginationService, int? page, int? pageSize, string? sortBy, string? sortOrder);

        /// <summary>
        /// Retrieves a Trainer by their unique TrainerId.
        /// </summary>
        Task<Trainer?> GetTrainerByIdAsync(int trainerId);
    }
    #endregion
    #region Implementation
    /// <inheritdoc />
    public class TrainerRepository : ITrainerRepository
    {
        private readonly AppDbContext _db;

        // Injects the application's DbContext
        public TrainerRepository(AppDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task AddTrainerAsync(Trainer trainer)
        {
            if (trainer == null)
                throw new ArgumentNullException(nameof(trainer));

            // Adds a new Trainer entity and saves changes asynchronously
            _db.Trainers.Add(trainer);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Trainer?> GetTrainerByUserIdAsync(int userId)
        {
            // Retrieves a Trainer entity by UserId, returns null if not found
            return await _db.Trainers.SingleOrDefaultAsync(t => t.UserId == userId);
        }

        /// <inheritdoc />
        public async Task<Trainer?> UpdateTrainerProfileAsync(int userId, UpdateTrainerProfileDto updatedTrainer)
        {
            if (updatedTrainer == null)
                throw new ArgumentNullException(nameof(updatedTrainer));

            // Updates an existing Trainer's profile fields and saves changes
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

        /// <inheritdoc />
        public async Task<(IEnumerable<Trainer> trainers, int total)> GetTrainersPagedAsync(IPaginationService paginationService, int? page, int? pageSize, string? sortBy, string? sortOrder)
        {
            if (paginationService == null)
                throw new ArgumentNullException(nameof(paginationService));

            // Retrieves a paginated list of Trainers with optional sorting, returns the list and total count
            var query = _db.Trainers.AsNoTracking();
            var total = await query.CountAsync();
            query = paginationService.ApplySorting(query, sortBy, sortOrder);
            query = paginationService.ApplyPagination(query, page, pageSize);
            return (await query.ToListAsync(), total);
        }

        /// <inheritdoc />
        public async Task<Trainer?> GetTrainerByIdAsync(int trainerId)
        {
            // Retrieves a Trainer by their unique TrainerId
            return await _db.Trainers.AsNoTracking().SingleOrDefaultAsync(t => t.Id == trainerId);
        }
    }
    #endregion





}