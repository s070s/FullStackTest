using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.ServiceUtilities.SeederService;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class UserSeederService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserSeederService> _logger;
        private readonly SeederHelperMethods _seederHelper;
        private readonly int _seedUsers;

        public UserSeederService(
            AppDbContext context,
            IConfiguration configuration,
            ILogger<UserSeederService> logger,
            SeederHelperMethods seederHelper,
            int seedUsers = 50)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _seederHelper = seederHelper;
            _seedUsers = seedUsers;
        }

        public async Task SeedUsersAsync(Dictionary<string, string> usernameMiddleNames)
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
                new User
                {
                    Username = "admin",
                    Email = adminEmail,
                    Role = UserRole.Admin,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    IsActive = true
                }
            };

            if (seedSampleData)
            {
                var generatedUsernames = new HashSet<string> { "admin" };
                int attemptsLimit = 10;

                for (int i = 1; i <= _seedUsers; i++)
                {
                    var role = _seederHelper.GetRandomUserRole();
                    string username;
                    string middleName;
                    int attempts = 0;

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
                    usernameMiddleNames[username] = middleName;
                }
            }
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {users.Count} users");
        }
    }
}