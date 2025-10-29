using Api.Data;
using Api.Services.Mapping;

namespace Api.Repositories.UnitOfWork
{

    #region Interface
    /// <summary>
    /// Interface for Unit of Work pattern.
    /// Exposes repositories and a method to persist changes.
    /// </summary>
    public interface IUnitOfWork
    {
        IClientRepository Clients { get; }    // Access to Client repository
        ITrainerRepository Trainers { get; }  // Access to Trainer repository
        IUserRepository Users { get; }        // Access to User repository
        Task<int> SaveChangesAsync();         // Persists all changes to the database
    }
    #endregion
    #region Implementation
    /// <inheritdoc/>
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

        ///<inheritdoc/>
        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
    #endregion

}