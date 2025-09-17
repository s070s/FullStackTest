using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public interface IClientRepository
    {
        Task AddClientAsync(Client client);
        Task<Client?> GetClientByUserIdAsync(int userId);
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
    }
}