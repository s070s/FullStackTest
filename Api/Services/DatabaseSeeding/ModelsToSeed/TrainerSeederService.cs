using Api.Data;
using Api.Models;
using Api.Models.Enums;
using Api.ServiceUtilities.SeederService;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class TrainerSeederService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TrainerSeederService> _logger;
        private readonly SeederHelperMethods _seederHelper;

        public TrainerSeederService(
            AppDbContext context,
            ILogger<TrainerSeederService> logger,
            SeederHelperMethods seederHelper)
        {
            _context = context;
            _logger = logger;
            _seederHelper = seederHelper;
        }

        public async Task SeedTrainersAsync(Dictionary<string, string> usernameMiddleNames)
        {
            if (await _context.Trainers.AnyAsync())
            {
                _logger.LogInformation("Trainers already exist, skipping trainer seeding");
                return;
            }

            var trainerUsers = await _context.Users
                .Where(u => u.Role == UserRole.Trainer)
                .ToListAsync();

            var trainers = new List<Trainer>();

            foreach (var user in trainerUsers)
            {
                var firstName = usernameMiddleNames.TryGetValue(user.Username, out var middle) ? middle : "John";
                var trainerSpecializations = _seederHelper.GetRandomTrainerSpecializations();
                var country = _seederHelper.GetRandomCountry();
                var city = _seederHelper.GetRandomCity(country);
                var address = _seederHelper.GetRandomStreetAddress(country);
                var state = country == "USA" ? _seederHelper.GetRandomState() : "CA";
                var weightKg = _seederHelper._random.Next(70, 100);
                var heightCm = _seederHelper._random.Next(160, 200);

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
                    BMR = weightKg * 22,
                    BMI = weightKg / Math.Pow(heightCm / 100.0, 2)
                };
                trainers.Add(trainer);
                user.TrainerProfile = trainer;
                _context.Users.Update(user);
            }

            if (trainers.Any())
            {
                _context.Trainers.AddRange(trainers);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {trainers.Count} trainers");
            }
        }
    }
}