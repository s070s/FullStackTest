using Api.Data;
using Api.Repositories;

namespace Api.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        IClientRepository Clients { get; }
        ITrainerRepository Trainers { get; }
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public IClientRepository Clients { get; }
        public ITrainerRepository Trainers { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Clients = new ClientRepository(db);
            Trainers = new TrainerRepository(db);
            Users = new UserRepository(db);
        }

        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}