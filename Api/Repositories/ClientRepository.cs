using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;

namespace Api.Repositories
{
    // Interface for client repository operations
    public interface IClientRepository
    {
        // Adds a new client to the database
        Task AddClientAsync(Client client);

        // Retrieves a client by their associated user ID
        Task<Client?> GetClientByUserIdAsync(int userId);

        // Updates a client's profile using the provided DTO and returns the updated client
        Task<Client?> UpdateClientProfileAsync(int userId, UpdateClientProfileDto updatedClient);
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

        // Adds a new client entity and saves changes asynchronously
        public async Task AddClientAsync(Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
        }

        // Retrieves a single client by user ID, or null if not found
        public async Task<Client?> GetClientByUserIdAsync(int userId)
        {
            return await _db.Clients.SingleOrDefaultAsync(c => c.UserId == userId);
        }

        // Updates an existing client's profile fields and saves changes
        // Returns the updated client, or null if not found
        public async Task<Client?> UpdateClientProfileAsync(int userId, UpdateClientProfileDto updatedClient)
        {
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
            existingClient.ExperienceLevel = updatedClient.ExperienceLevel;

            await _db.SaveChangesAsync();
            return existingClient;
        }
    }
}