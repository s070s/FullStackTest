using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.ServiceUtilities.SeederService;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ClientSeederService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientSeederService> _logger;
        private readonly SeederHelperMethods _seederHelper;

        public ClientSeederService(
            AppDbContext context,
            ILogger<ClientSeederService> logger,
            SeederHelperMethods seederHelper)
        {
            _context = context;
            _logger = logger;
            _seederHelper = seederHelper;
        }

        public async Task SeedClientsAsync(Dictionary<string, string> usernameMiddleNames)
        {
            if (await _context.Clients.AnyAsync())
            {
                _logger.LogInformation("Clients already exist, skipping client seeding");
                return;
            }

            var clientUsers = await _context.Users
                .Where(u => u.Role == UserRole.Client)
                .ToListAsync();

            var clients = new List<Client>();

            foreach (var user in clientUsers)
            {
                var firstName = usernameMiddleNames.TryGetValue(user.Username, out var middle) ? middle : "John";
                var clientExperience = _seederHelper.GetRandomClientExperience();
                var weightKg = _seederHelper._random.Next(70, 100);
                var heightCm = _seederHelper._random.Next(160, 200);
                var dob = _seederHelper.GetRandomDateOfBirth();
                var intensity = _seederHelper.GetClientIntensityLevel(dob, weightKg, heightCm);
                var country = _seederHelper.GetRandomCountry();
                var state = country == "USA" ? _seederHelper.GetRandomState() : "CA";

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
                    BMR = weightKg * 22,
                    BMI = weightKg / Math.Pow(heightCm / 100.0, 2)
                };
                clients.Add(client);
                user.ClientProfile = client;
                _context.Users.Update(user);
            }
            if (clients.Any())
            {
                _context.Clients.AddRange(clients);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {clients.Count} clients");
            }
        }
    }
}