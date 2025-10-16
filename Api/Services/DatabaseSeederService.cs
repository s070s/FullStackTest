using Microsoft.EntityFrameworkCore;
using Api.ServiceUtilities.SeederService;
using Api.Data;
using Api.Models;
using Api.Models.Enums;


//Todo:Refactor: Break into smaller services (e.g., UserSeederService, TrainerSeederService, ClientSeederService)
namespace Api.Services
{
    public class DatabaseSeederService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseSeederService> _logger;
        private readonly SeederHelperMethods _seederHelper;
        // Stores generated middle names for each username to use in profile seeding (Trainer/Client)
        private Dictionary<string, string> _usernameMiddleNames = new();
        // Number of sample users to seed when SeedSampleData is enabled
        int seedUsers = 50;
        public DatabaseSeederService(
            AppDbContext context,
            IConfiguration configuration,
            ILogger<DatabaseSeederService> logger,
            SeederHelperMethods seederHelper)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _seederHelper = seederHelper;
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
                // Ensure database exists
                await _context.Database.EnsureCreatedAsync();
                // Seed in correct order (users first, then profiles)
                await SeedUsersAsync();
                await SeedTrainersAndClientsAsync();

                // Todo:Seed reference data(e.g., exercise types, equipment, etc.)
                await SeedReferenceDataAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }


        #region Seed Users and Profiles
        /// <summary>
        /// Seeds the Users table with an admin user and, if enabled, sample users for development/testing.
        /// </summary>
        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Users already exist, skipping user seeding");
                return;
            }

            var adminPassword = _configuration["AdminUser:Password"] ?? "Admin123!";
            var adminEmail = _configuration["AdminUser:Email"] ?? "admin@fitnessapp.com";
            var seedSampleData = _configuration.GetValue<bool>("SeedSampleData", false);

            var users = new List<User>
            {
                // Always seed the admin user
                new User
                {
                    Username = "admin",
                    Email = adminEmail,
                    Role = UserRole.Admin,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    IsActive = true
                }
            };

            // Add sample users only if SeedSampleData is enabled (typically in development/testing)
            if (seedSampleData)
            {
                var generatedUsernames = new HashSet<string> { "admin" };
                int attemptsLimit = 10;

                for (int i = 1; i <= seedUsers; i++)
                {
                    var role = _seederHelper.GetRandomUserRole();
                    string username;
                    string middleName;
                    int attempts = 0;

                    // Ensure unique username
                    do
                    {
                        (username, middleName) = _seederHelper.GenerateRandomUsernameWithMiddle(role.ToString());
                        attempts++;
                    }
                    while (generatedUsernames.Contains(username) && attempts < attemptsLimit);

                    if (generatedUsernames.Contains(username))
                    {
                        _logger.LogWarning($"Could not generate unique username after {attemptsLimit} attempts, skipping user {i}");
                        continue;
                    }

                    generatedUsernames.Add(username);
                    var mail = _seederHelper.GenerateRandomEmail(username);
                    var testPassword = _seederHelper.GeneratePassword(username);

                    users.Add(new User
                    {
                        Username = username,
                        Email = mail,
                        Role = role,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(testPassword),
                        IsActive = true,
                    });
                    _usernameMiddleNames[username] = middleName;
                }
            }
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {users.Count} users");
        }
        /// <summary>
        /// Seeds Trainer and Client profiles for sample users if 'SeedSampleData' is enabled in configuration.
        /// Should be called after users are seeded.
        /// </summary>
        private async Task SeedTrainersAndClientsAsync()
        {
            var seedSampleData = _configuration.GetValue<bool>("SeedSampleData", false);
            if (!seedSampleData) return;

            await SeedTrainersAsync();
            await SeedClientsAsync();
        }

        /// <summary>
        /// Seeds Trainer profiles for users with the Trainer role.
        /// </summary>
        private async Task SeedTrainersAsync()
        {
            // Check if trainers already exist to avoid duplicate seeding
            if (await _context.Trainers.AnyAsync())
            {
                _logger.LogInformation("Trainers already exist, skipping trainer seeding");
                return;
            }

            // Get all users with the Trainer role
            var trainerUsers = await _context.Users
                .Where(u => u.Role == UserRole.Trainer)
                .ToListAsync();

            var trainers = new List<Trainer>();

            foreach (var user in trainerUsers)
            {
                // Use the generated middle name as first name, fallback to "John"
                var firstName = _usernameMiddleNames.TryGetValue(user.Username, out var middle) ? middle : "John";
                // Randomly assign 1-3 specializations
                var trainerSpecializations = _seederHelper.GetRandomTrainerSpecializations();
                // Generate random location and address data
                var country = _seederHelper.GetRandomCountry();
                var city = _seederHelper.GetRandomCity(country);
                var address = _seederHelper.GetRandomStreetAddress(country);
                // Default state, override if USA
                var state = "CA";
                if (country == "USA")
                {
                    state = _seederHelper.GetRandomState();
                }
                // Generate random weight and height
                var weightKg = _seederHelper._random.Next(70, 100);
                var heightCm = _seederHelper._random.Next(160, 200);
                // Create Trainer profile
                var trainer = new Trainer
                {
                    UserId = user.Id,
                    Bio = _seederHelper.GetRandomTrainerBio(trainerSpecializations.First()),
                    Specializations = trainerSpecializations,
                    FirstName = firstName,
                    LastName = _seederHelper.GetRandomLastName(),
                    DateOfBirth = _seederHelper.GetRandomDateOfBirth(),
                    PhoneNumber = _seederHelper.GenerateRandomPhoneNumber(),
                    Address = address,
                    City = city,
                    State = state,
                    ZipCode = _seederHelper.GetRandomZipCode(),
                    Country = country,
                    Weight = weightKg,
                    Height = heightCm,
                    BMR = weightKg * 22, // Simplified BMR calculation
                    BMI = weightKg / Math.Pow(heightCm / 100.0, 2) // BMI calculation
                };
                trainers.Add(trainer);
                user.TrainerProfile = trainer;
                _context.Users.Update(user);
            }

            // Add all generated trainers to the database
            if (trainers.Any())
            {
                _context.Trainers.AddRange(trainers);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {trainers.Count} trainers");
            }
        }

        /// <summary>
        /// Seeds Client profiles for users with the Client role.
        /// </summary>
        private async Task SeedClientsAsync()
        {
            // Skip seeding if clients already exist

            if (await _context.Clients.AnyAsync())
            {
                _logger.LogInformation("Clients already exist, skipping client seeding");
                return;
            }

            // Get all users with the Client role
            var clientUsers = await _context.Users
                .Where(u => u.Role == UserRole.Client)
                .ToListAsync();

            var clients = new List<Client>();

            foreach (var user in clientUsers)
            {
                // Use generated middle name as first name, fallback to "John"
                var firstName = _usernameMiddleNames.TryGetValue(user.Username, out var middle) ? middle : "John";
                var clientExperience = _seederHelper.GetRandomClientExperience();
                var weightKg = _seederHelper._random.Next(70, 100);
                var heightCm = _seederHelper._random.Next(160, 200);
                var dob = _seederHelper.GetRandomDateOfBirth();
                var intensity = _seederHelper.GetClientIntensityLevel(dob, weightKg, heightCm); // Default intensity
                var country = _seederHelper.GetRandomCountry();
                var state = "CA";
                if (country == "USA")
                {
                    state = _seederHelper.GetRandomState();
                }
                // Create Client profile with randomized data
                var client = new Client
                {
                    UserId = user.Id,
                    Bio = _seederHelper.GetRandomClientBio(firstName, clientExperience, country),
                    ExperienceLevel = clientExperience,
                    PreferredIntensityLevel = intensity,
                    FirstName = firstName,
                    LastName = _seederHelper.GetRandomLastName(),
                    DateOfBirth = dob,
                    PhoneNumber = _seederHelper.GenerateRandomPhoneNumber(),
                    Address = _seederHelper.GetRandomStreetAddress(country),
                    City = _seederHelper.GetRandomCity(country),
                    State = state,
                    ZipCode = _seederHelper.GetRandomZipCode(),
                    Country = country,
                    Weight = weightKg,
                    Height = heightCm,
                    BMR = weightKg * 22, // Simplified BMR calculation
                    BMI = weightKg / Math.Pow(heightCm / 100.0, 2) // BMI calculation
                };
                clients.Add(client);
                user.ClientProfile = client;
                _context.Users.Update(user);
            }
            // Add all generated clients to the database
            if (clients.Any())
            {
                _context.Clients.AddRange(clients);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {clients.Count} clients");
            }
        }

        #endregion

        /// <summary>
        /// Seeds reference data such as exercise types and equipment.
        /// </summary>
        private async Task SeedReferenceDataAsync()
        {
            // Todo:Seed reference data(e.g., exercise types, equipment, etc.)
            await Task.CompletedTask;
        }
    }
}