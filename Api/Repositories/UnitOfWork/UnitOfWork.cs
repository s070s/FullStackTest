using Api.Data;
using Api.Services.Mapping;

namespace Api.Repositories.UnitOfWork
{
    // Interface for Unit of Work pattern.
    // Exposes repositories and a method to persist changes.
    public interface IUnitOfWork
    {
        IClientRepository Clients { get; }    // Access to Client repository
        ITrainerRepository Trainers { get; }  // Access to Trainer repository
        IUserRepository Users { get; }        // Access to User repository
        Task<int> SaveChangesAsync();         // Persists all changes to the database
    }

    // Concrete implementation of the Unit of Work pattern.
    // Manages repositories and coordinates saving changes.
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db; // EF Core database context
        private readonly IModelToDtoMapper _mapper;

        public IClientRepository Clients { get; }   // Client repository instance
        public ITrainerRepository Trainers { get; } // Trainer repository instance
        public IUserRepository Users { get; }       // User repository instance

        // Constructor injects the database context and initializes repositories.
        public UnitOfWork(AppDbContext db,IModelToDtoMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            Clients = new ClientRepository(db);
            Trainers = new TrainerRepository(db);
            Users = new UserRepository(db, _mapper);
        }

        // Saves all changes made in the context to the database asynchronously.
        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}