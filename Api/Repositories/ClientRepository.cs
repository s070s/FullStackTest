using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Dtos;

namespace Api.Repositories
{
    public interface IClientRepository
    {
        Task AddClientAsync(Client client);
        Task<Client?> GetClientByUserIdAsync(int userId);
        Task<Client?> UpdateClientProfileAsync(int userId, UpdateClientProfileDto updatedClient);
    }

    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _db;
        public ClientRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddClientAsync(Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
        }

        public async Task<Client?> GetClientByUserIdAsync(int userId)
        {
            return await _db.Clients.SingleOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Client?> UpdateClientProfileAsync(int userId, UpdateClientProfileDto updatedClient)
        {
            var existingClient = await _db.Clients.SingleOrDefaultAsync(c => c.UserId == userId);
            if (existingClient == null) return null;

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