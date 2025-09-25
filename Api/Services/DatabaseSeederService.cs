using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.Models.Enums;



namespace Api.Services
{
    public class DatabaseSeederService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseSeederService> _logger;
        private readonly Random _random = new();
        // Stores generated middle names for each username to use in profile seeding (Trainer/Client)
        private Dictionary<string, string> _usernameMiddleNames = new();
        // Number of sample users to seed when SeedSampleData is enabled
        int seedUsers = 20;
        public DatabaseSeederService(
            AppDbContext context,
            IConfiguration configuration,
            ILogger<DatabaseSeederService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
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
                for (int i = 1; i <= seedUsers; i++)
                {
                    var role = GetRandomUserRole();
                    var (username, middleName) = GenerateRandomUsernameWithMiddle(role.ToString());
                    var mail = GenerateRandomEmail(username);
                    var testPassword = GeneratePassword(username);
                    users.Add(new User
                    {
                        Username = username,
                        Email = mail,
                        Role = role,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(testPassword),
                        IsActive = true
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
                var trainerSpecializations = GetRandomTrainerSpecializations();
                // Generate random location and address data
                var country = GetRandomCountry();
                var city = GetRandomCity(country);
                var address = GetRandomStreetAddress(country);
                // Default state, override if USA
                var state = "CA";
                if (country == "USA")
                {
                    state = GetRandomState();
                }
                // Generate random weight and height
                var weightKg = _random.Next(70, 100);
                var heightCm = _random.Next(160, 200);
                // Create Trainer profile
                var trainer = new Trainer
                {
                    UserId = user.Id,
                    Bio = GetRandomTrainerBio(trainerSpecializations.First()),
                    Specializations = trainerSpecializations,
                    FirstName = firstName,
                    LastName = GetRandomLastName(),
                    DateOfBirth = GetRandomDateOfBirth(),
                    PhoneNumber = GenerateRandomPhoneNumber(country),
                    Address = address,
                    City = city,
                    State = state,
                    ZipCode = GetRandomZipCode(),
                    Country = country,
                    Weight = weightKg,
                    Height = heightCm,
                    BMR = weightKg * 22, // Simplified BMR calculation
                    BMI = weightKg / Math.Pow(heightCm / 100.0, 2) // BMI calculation
                };
                trainers.Add(trainer);
            }

            // Add all generated trainers to the database
            if (trainers.Any())
            {
                _context.Trainers.AddRange(trainers);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {trainers.Count} trainers");
            }
        }

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
                var clientExperience = GetRandomClientExperience();
                var weightKg = _random.Next(70, 100);
                var heightCm = _random.Next(160, 200);
                var dob = GetRandomDateOfBirth();
                var intensity = GetClientIntensityLevel(dob, weightKg, heightCm); // Default intensity
                var country = GetRandomCountry();
                var state = "CA";
                if (country == "USA")
                {
                    state = GetRandomState();
                }
                // Create Client profile with randomized data
                var client = new Client
                {
                    UserId = user.Id,
                    Bio = GetRandomClientBio(firstName, clientExperience, country),
                    ExperienceLevel = clientExperience,
                    PreferredIntensityLevel = intensity,
                    FirstName = firstName,
                    LastName = GetRandomLastName(),
                    DateOfBirth = dob,
                    PhoneNumber = GenerateRandomPhoneNumber(country),
                    Address = GetRandomStreetAddress(country),
                    City = GetRandomCity(country),
                    State = state,
                    ZipCode = GetRandomZipCode(),
                    Country = country,
                    Weight = weightKg,
                    Height = heightCm,
                    BMR = weightKg * 22, // Simplified BMR calculation
                    BMI = weightKg / Math.Pow(heightCm / 100.0, 2) // BMI calculation
                };
                clients.Add(client);
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

        private async Task SeedReferenceDataAsync()
        {
            // Todo:Seed reference data(e.g., exercise types, equipment, etc.)
            await Task.CompletedTask;
        }

        #region Helper Methods

        #region User Generation

        /// <summary>
        /// Generates a random username and a corresponding "middle name" (used as first name in profiles)
        /// based on the user role ("Trainer" or "Client").
        /// Returns a tuple: (username, middleName).
        /// </summary>
        private (string Username, string MiddleName) GenerateRandomUsernameWithMiddle(string role)
        {
            switch (role)
            {
                case "Trainer":
                    var trainerPrefixes = new[] { "fit", "strong", "power", "muscle", "train" };
                    var trainerMiddlesNames = new[] { "John", "Alex", "Chris", "Sam", "Jordan", "Maria", "Helen", "Kate", "Alice" };
                    var trainerSuffixes = new[] { "101", "pro", "coach", "expert" };
                    var middleT = trainerMiddlesNames[_random.Next(trainerMiddlesNames.Length)];
                    var usernameT = $"{trainerPrefixes[_random.Next(trainerPrefixes.Length)]}{middleT}{trainerSuffixes[_random.Next(trainerSuffixes.Length)]}";
                    return (usernameT, middleT);
                case "Client":
                    var clientPrefixes = new[] { "fit", "health", "well", "active", "move" };
                    var clientMiddlesNames = new[] { "Mike", "Sara", "Tom", "Emma", "Lucy", "David", "Nina", "Liam", "Olivia" };
                    var clientSuffixes = new[] { "2023", "go", "life", "now" };
                    var middleC = clientMiddlesNames[_random.Next(clientMiddlesNames.Length)];
                    var usernameC = $"{clientPrefixes[_random.Next(clientPrefixes.Length)]}{middleC}{clientSuffixes[_random.Next(clientSuffixes.Length)]}";
                    return (usernameC, middleC);
                default:
                    return ($"user{_random.Next(1, 99)}", "User");
            }
        }
        /// <summary>
        /// Generates a random email address for a given username using a random fake domain.
        /// </summary>
        private string GenerateRandomEmail(string username)
        {
            var domains = new[] { "geemail.com", "hawtmail.com", "yoohoo.com" };
            return $"{username.ToLower()}@{domains[_random.Next(domains.Length)]}";
        }
        /// <summary>
        /// Returns a random UserRole value from the UserRole enum.
        /// </summary>
        private UserRole GetRandomUserRole()
        {
            var roles = Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();
            return roles[_random.Next(roles.Count)];
        }
        /// <summary>
        /// Generates a simple password for a given username (for sample users only).
        /// </summary>
        private string GeneratePassword(string username)
        {
            return $"{username}1234567";
        }

        #endregion

        #region Profiles:Client/Trainer Generation
        /// <summary>
        /// Returns a random ClientExperience value from the ClientExperience enum.
        /// </summary>
        private ClientExperience GetRandomClientExperience()
        {
            var levels = Enum.GetValues(typeof(ClientExperience)).Cast<ClientExperience>().ToList();
            return levels[_random.Next(levels.Count)];
        }
        /// <summary>
        /// Determines a client's preferred intensity level based on age, weight, and height.
        /// Simplified logic: Younger and fitter clients prefer higher intensity.
        /// </summary>
        private IntensityLevel GetClientIntensityLevel(DateTime dob, int weight, int height)
        {
            var age = DateTime.Now.Year - dob.Year;
            if (dob > DateTime.Now.AddYears(-age)) age--;

            if (age < 30 && weight < 180 && height > 65)
                return IntensityLevel.High;
            else if (age < 50)
                return IntensityLevel.Medium;
            else
                return IntensityLevel.Low;
        }
        /// <summary>
        /// Generates a random client bio based on first name, experience level, and country.
        /// Combines a random introduction with an experience-appropriate sentence.
        /// </summary>
        private string GetRandomClientBio(string FirstName, ClientExperience experience, string Country)
        {
            var nameStringIntroductionArray = new[] { $"Hi, I'm {FirstName} from {Country}", $"Hello! My name is {FirstName} and I live in {Country}", $"Hey there, I'm {FirstName} from {Country}" };
            switch (experience)
            {
                case ClientExperience.Beginner:
                    var beginnerBios = new[]
                    {
                        ". I'm new to fitness and excited to start my journey towards a healthier lifestyle.",
                        ". As a beginner, I'm looking forward to learning the basics and building a strong foundation.",
                        ". I'm eager to explore different workouts and find what works best for me."
                    };
                    return $"{nameStringIntroductionArray[_random.Next(nameStringIntroductionArray.Length)]}{beginnerBios[_random.Next(beginnerBios.Length)]}";
                case ClientExperience.Occasional:
                    var occasionalBios = new[]
                    {
                        ". I've been working out for a while and want to take my fitness to the next level.",
                        ". With some experience under my belt, I'm ready to challenge myself and achieve new goals.",
                        ". I'm looking to refine my techniques and push my limits further."
                    };
                    return $"{nameStringIntroductionArray[_random.Next(nameStringIntroductionArray.Length)]}{occasionalBios[_random.Next(occasionalBios.Length)]}";
                case ClientExperience.Regular:
                    var regularBios = new[]
                    {
                        ". As a regular fitness enthusiast, I'm committed to maintaining a consistent workout routine.",
                        ". I have a solid fitness background and am always looking for ways to improve my performance.",
                        ". I'm passionate about staying active and exploring new fitness trends."
                    };
                    return $"{nameStringIntroductionArray[_random.Next(nameStringIntroductionArray.Length)]}{regularBios[_random.Next(regularBios.Length)]}";
                case ClientExperience.Athlete:
                    var athleteBios = new[]
                    {
                        ". With advanced experience, I'm focused on optimizing my training and achieving peak performance.",
                        ". I'm dedicated to pushing my boundaries and mastering complex workout techniques.",
                        ". As an athlete, I'm always seeking new challenges and opportunities for growth."
                    };
                    return $"{nameStringIntroductionArray[_random.Next(nameStringIntroductionArray.Length)]}{athleteBios[_random.Next(athleteBios.Length)]}";
                default:
                    return $"{nameStringIntroductionArray[_random.Next(nameStringIntroductionArray.Length)]}. I'm passionate about fitness and excited to embark on this journey.";
            }

        }
        /// <summary>
        /// Generates a random trainer bio based on specialization.
        /// Combines a random adjective, specialization, and filler for variety.
        /// </summary>
        private string GetRandomTrainerBio(TrainerSpecialization specialization)
        {
            var adjectives = new[] { "Dedicated", "Experienced", "Passionate", "Certified", "Motivated" };
            var fillers = new[] { "fitness", "health", "wellness", "strength", "conditioning" };
            var bios = new[]
            {
                $"{adjectives[_random.Next(adjectives.Length)]} trainer specializing in {specialization.ToString().ToLower()} and overall {fillers[_random.Next(fillers.Length)]}.",
                $"Helping clients achieve their goals with personalized {specialization.ToString().ToLower()} programs and expert guidance.",
                $"Committed to improving lives through effective {specialization.ToString().ToLower()} coaching and holistic health approaches."
            };
            return bios[_random.Next(bios.Length)];
        }
        /// <summary>
        /// Returns a random list of TrainerSpecialization values (1 to 3 specializations per trainer).
        /// </summary>
        private List<TrainerSpecialization> GetRandomTrainerSpecializations()
        {
            var specializations = Enum.GetValues(typeof(TrainerSpecialization)).Cast<TrainerSpecialization>().ToList();
            int count = _random.Next(1, 4); // Each trainer can have 1 to 3 specializations
            return specializations.OrderBy(x => _random.Next()).Take(count).ToList();
        }
        /// <summary>
        /// Returns a random last name from a predefined list.
        /// </summary>
        private string GetRandomLastName()
        {
            var lastNames = new[] { "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas", "Jackson", "White" };
            return lastNames[_random.Next(lastNames.Length)];
        }
        /// <summary>
        /// Returns a random Date of Birth.
        /// </summary>
        private DateTime GetRandomDateOfBirth()
        {
            int year = _random.Next(1950, 2005); // Age between ~18 and ~73
            int month = _random.Next(1, 13);
            int day = _random.Next(1, 28); // Simplified to avoid month length issues
            return new DateTime(year, month, day);
        }
                /// <summary>
        /// Returns a random country from a predefined list.
        /// </summary>
        private string GetRandomCountry()
        {
            var countries = new[] { "USA", "Canada", "UK", "Australia", "Germany", "France", "Italy" };
            return countries[_random.Next(countries.Length)];
        }
                /// <summary>
        /// Returns a random city.
        /// </summary>
        private string GetRandomCity(string Country)
        {
            switch (Country)
            {
                case "USA":
                    var usCities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix" };
                    return usCities[_random.Next(usCities.Length)];
                case "Canada":
                    var caCities = new[] { "Toronto", "Vancouver", "Montreal", "Calgary", "Ottawa" };
                    return caCities[_random.Next(caCities.Length)];
                case "UK":
                    var ukCities = new[] { "London", "Manchester", "Birmingham", "Glasgow", "Liverpool" };
                    return ukCities[_random.Next(ukCities.Length)];
                case "Australia":
                    var auCities = new[] { "Sydney", "Melbourne", "Brisbane", "Perth", "Adelaide" };
                    return auCities[_random.Next(auCities.Length)];
                case "Germany":
                    var deCities = new[] { "Berlin", "Munich", "Frankfurt", "Hamburg", "Cologne" };
                    return deCities[_random.Next(deCities.Length)];
                case "France":
                    var frCities = new[] { "Paris", "Marseille", "Lyon", "Toulouse", "Nice" };
                    return frCities[_random.Next(frCities.Length)];
                case "Italy":
                    var itCities = new[] { "Rome", "Milan", "Naples", "Turin", "Palermo" };
                    return itCities[_random.Next(itCities.Length)];
                default:
                    return "Unknown City";
            }
        }
                /// <summary>
        /// Returns a random phone number.
        /// </summary>
        private string GenerateRandomPhoneNumber(string country)
        {
            // Simplified accurate phone generation
            return country switch
            {
                "USA" => $"{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                "Canada" => $"{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                "UK" => $"+44 {_random.Next(10, 99)} {_random.Next(1000, 9999)} {_random.Next(1000, 9999)}",
                "Australia" => $"+61 {_random.Next(4, 5)} {_random.Next(1000, 9999)} {_random.Next(1000, 9999)}",
                "Germany" => $"+49 {_random.Next(100, 999)} {_random.Next(1000, 9999)} {_random.Next(1000, 9999)}",
                "France" => $"+33 {_random.Next(1, 9)} {_random.Next(10, 99)} {_random.Next(1000, 9999)}",
                "Italy" => $"+39 {_random.Next(100, 999)} {_random.Next(1000, 9999)} {_random.Next(1000, 9999)}",
                _ => $"{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}"
            };
        }
                /// <summary>
        /// Returns a random state if the country is USA.
        /// </summary>
        private string GetRandomState()
        {
            var states = new[] { "CA", "NY", "TX", "FL", "IL", "PA", "OH", "MI", "GA", "NC" };
            return states[_random.Next(states.Length)];
        }
        /// <summary>
        /// Returns a random street address.
        /// </summary>
        private string GetRandomStreetAddress(string country)
        {
            switch (country)
            {
                case "USA":
                case "Canada":
                    var streetNumbers = _random.Next(100, 9999);
                    var streetNames = new[] { "Main St", "Oak St", "Pine St", "Maple Ave", "Cedar Rd" };
                    return $"{streetNumbers} {streetNames[_random.Next(streetNames.Length)]}";
                case "UK":
                    var ukStreetNumbers = _random.Next(1, 200);
                    var ukStreetNames = new[] { "High St", "Station Rd", "Church St", "London Rd", "Victoria St" };
                    return $"{ukStreetNumbers} {ukStreetNames[_random.Next(ukStreetNames.Length)]}";
                case "Australia":
                    var auStreetNumbers = _random.Next(1, 300);
                    var auStreetNames = new[] { "George St", "King St", "Queen St", "Elizabeth St", "Market St" };
                    return $"{auStreetNumbers} {auStreetNames[_random.Next(auStreetNames.Length)]}";
                case "Germany":
                    var deStreetNumbers = _random.Next(1, 500);
                    var deStreetNames = new[] { "Hauptstrasse", "Bahnhofstrasse", "Schulstrasse", "Gartenweg", "Dorfstrasse" };
                    return $"{deStreetNumbers} {deStreetNames[_random.Next(deStreetNames.Length)]}";
                case "France":
                    var frStreetNumbers = _random.Next(1, 400);
                    var frStreetNames = new[] { "Rue de la Paix", "Avenue des Champs", "Boulevard Saint-Michel", "Rue du Faubourg", "Place de la République" };
                    return $"{frStreetNumbers} {frStreetNames[_random.Next(frStreetNames.Length)]}";
                case "Italy":
                    var itStreetNumbers = _random.Next(1, 600);
                    var itStreetNames = new[] { "Via Roma", "Corso Vittorio", "Piazza Navona", "Via Milano", "Largo Augusto" };
                    return $"{itStreetNumbers} {itStreetNames[_random.Next(itStreetNames.Length)]}";
                default:
                    return "123 Fitness St";
            }
        }
        /// <summary>
        /// Returns a random zip code.
        /// </summary>
        private string GetRandomZipCode()
        {
            return $"{_random.Next(10000, 99999)}";
        }

        #endregion



        #endregion
    }
}