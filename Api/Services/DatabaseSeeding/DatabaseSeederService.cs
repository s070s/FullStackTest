using Api.ServiceUtilities.SeederService;
using Api.Data;


namespace Api.Services
{
    public class DatabaseSeederService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseSeederService> _logger;
        private readonly SeederHelperMethods _seederHelper;

        // Seeder services from DI
        private readonly UserSeederService _userSeeder;
        private readonly TrainerSeederService _trainerSeeder;
        private readonly ClientSeederService _clientSeeder;

        public DatabaseSeederService(
            AppDbContext context,
            IConfiguration configuration,
            ILogger<DatabaseSeederService> logger,
            SeederHelperMethods seederHelper,
            UserSeederService userSeeder,
            TrainerSeederService trainerSeeder,
            ClientSeederService clientSeeder)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _seederHelper = seederHelper;
            _userSeeder = userSeeder;
            _trainerSeeder = trainerSeeder;
            _clientSeeder = clientSeeder;
        }

        /// <summary>
        /// Seeds the database with initial data in the correct order:
        /// 1. Ensures the database is created.
        /// 2. Seeds data (users,trainer/client profiles etc)
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();

                var usernameMiddleNames = new Dictionary<string, string>();
                await _userSeeder.SeedUsersAsync(usernameMiddleNames);

                var seedSampleData = _configuration.GetValue<bool>("SeedSampleData", false);
                if (seedSampleData)
                {
                    await _trainerSeeder.SeedTrainersAsync(usernameMiddleNames);
                    await _clientSeeder.SeedClientsAsync(usernameMiddleNames);
                }

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}