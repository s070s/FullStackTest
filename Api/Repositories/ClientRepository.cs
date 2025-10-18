using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;
using Api.Models.Enums;

namespace Api.Repositories
{
    // Interface for client repository operations
    public interface IClientRepository
    {
        /// <summary>
        /// Adds a new client to the database.
        /// </summary>
        Task AddClientAsync(Client client);

        /// <summary>
        /// Retrieves a client by their associated user ID.
        /// </summary>
        Task<Client?> GetClientByUserIdAsync(int userId);

        /// <summary>
        /// Updates a client's profile using the provided DTO and returns the updated client.
        /// </summary>
        Task<Client?> UpdateClientProfileAsync(int userId, UpdateClientProfileDto updatedClient);
        /// <summary>
        /// As a Client subscribe to a Trainer
        /// </summary>
        Task<bool> SubscribeToTrainerAsync(int clientId, int trainerId);

        /// <summary>
        /// Unsubscribes a client from a trainer.
        /// </summary>
        Task<bool> UnsubscribeFromTrainerAsync(int clientId, int trainerId);
    }

    // Implementation of the client repository
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _db;

        // Injects the application's database context
        public ClientRepository(AppDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task AddClientAsync(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            // Adds a new client entity and saves changes asynchronously
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Client?> GetClientByUserIdAsync(int userId)
        {
            // FIX: Include Trainers so subscriptions are loaded!
            return await _db.Clients
                .Include(c => c.Trainers)
                .SingleOrDefaultAsync(c => c.UserId == userId);
        }

        /// <inheritdoc />
        public async Task<Client?> UpdateClientProfileAsync(int userId, UpdateClientProfileDto updatedClient)
        {
            if (updatedClient == null)
                throw new ArgumentNullException(nameof(updatedClient));

            // Updates an existing client's profile fields and saves changes
            var existingClient = await _db.Clients.SingleOrDefaultAsync(c => c.UserId == userId);
            if (existingClient == null) return null;

            // Update fields from DTO
            existingClient.FirstName = updatedClient.FirstName;
            existingClient.LastName = updatedClient.LastName;
            existingClient.Bio = updatedClient.Bio;
            existingClient.DateOfBirth = updatedClient.DateOfBirth;
            existingClient.Height = updatedClient.Height;
            existingClient.Weight = updatedClient.Weight;
            existingClient.PhoneNumber = updatedClient.PhoneNumber;
            existingClient.Country = updatedClient.Country;
            existingClient.City = updatedClient.City;
            existingClient.Address = updatedClient.Address;
            existingClient.ZipCode = updatedClient.ZipCode;
            existingClient.State = updatedClient.State;
            existingClient.ExperienceLevel = updatedClient.ExperienceLevel ?? ClientExperience.Beginner;

            await _db.SaveChangesAsync();
            return existingClient;
        }
        /// <inheritdoc />
        public async Task<bool> SubscribeToTrainerAsync(int clientId, int trainerId)
        {
            var client = await _db.Clients
                .Include(c => c.Trainers)
                .SingleOrDefaultAsync(c => c.Id == clientId);
            var trainer = await _db.Trainers.FindAsync(trainerId);
            if (client == null || trainer == null)
                return false;
            if (!client.Trainers.Contains(trainer))
            {
                client.Trainers.Add(trainer);
                await _db.SaveChangesAsync();
            }
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> UnsubscribeFromTrainerAsync(int clientId, int trainerId)
        {
            var client = await _db.Clients
                .Include(c => c.Trainers)
                .SingleOrDefaultAsync(c => c.Id == clientId);
            var trainer = await _db.Trainers.FindAsync(trainerId);
            if (client == null || trainer == null)
                return false;
            if (client.Trainers.Contains(trainer))
            {
                client.Trainers.Remove(trainer);
                await _db.SaveChangesAsync();
            }
            return true;
        }
    }
}