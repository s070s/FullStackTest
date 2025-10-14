using Api.Models.Enums;


namespace Api.ServiceUtilities.SeederService
{
    public class SeederHelperMethods
    {
        public readonly Random _random = new();
        private readonly TrainerData _trainerData;
        private readonly ClientData _clientData;
        private readonly IdInfoData _idInfoData;
        private readonly ContactInfoData _contactInfoData;


        public SeederHelperMethods(
           TrainerData trainerData,
           ClientData clientData,
           IdInfoData idInfoData,
           ContactInfoData contactInfoData)
        {
            _trainerData = trainerData;
            _clientData = clientData;
            _idInfoData = idInfoData;
            _contactInfoData = contactInfoData;
        }

        #region Helper Methods

        #region User Generation

        /// <summary>
        /// Generates a random username and a corresponding "middle name" (used as first name in profiles)
        /// based on the user role ("Trainer" or "Client").
        /// Returns a tuple: (username, middleName).
        /// </summary>
        public (string Username, string MiddleName) GenerateRandomUsernameWithMiddle(string role)
        {
            switch (role)
            {
                case "Trainer":
                    var middleT = _trainerData.TrainerMiddleNames[_random.Next(_trainerData.TrainerMiddleNames.Length)];
                    var usernameT = $"{_trainerData.TrainerPrefixes[_random.Next(_trainerData.TrainerPrefixes.Length)]}{middleT}{_trainerData.TrainerSuffixes[_random.Next(_trainerData.TrainerSuffixes.Length)]}";
                    return (usernameT, middleT);
                case "Client":
                    var middleC = _clientData.ClientMiddleNames[_random.Next(_clientData.ClientMiddleNames.Length)];
                    var usernameC = $"{_clientData.ClientPrefixes[_random.Next(_clientData.ClientPrefixes.Length)]}{middleC}{_clientData.ClientSuffixes[_random.Next(_clientData.ClientSuffixes.Length)]}";
                    return (usernameC, middleC);
                default:
                    return ($"user{_random.Next(1, 99)}", "User");
            }
        }
        /// <summary>
        /// Generates a random email address for a given username using a random fake domain.
        /// </summary>
        public string GenerateRandomEmail(string username)
        {
            return $"{username.ToLower()}@{_contactInfoData.EmailDomains[_random.Next(_contactInfoData.EmailDomains.Length)]}";
        }
        /// <summary>
        /// Returns a random UserRole value from the UserRole enum.
        /// </summary>
        public UserRole GetRandomUserRole()
        {
            var roles = Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();
            return roles[_random.Next(roles.Count)];
        }
        /// <summary>
        /// Generates a simple password for a given username (for sample users only).
        /// </summary>
        public string GeneratePassword(string username)
        {
            return $"{username}1234567";
        }

        #endregion

        #region Profiles:Client/Trainer Generation
        /// <summary>
        /// Returns a random ClientExperience value from the ClientExperience enum.
        /// </summary>
        public ClientExperience GetRandomClientExperience()
        {
            var levels = Enum.GetValues(typeof(ClientExperience)).Cast<ClientExperience>().ToList();
            return levels[_random.Next(levels.Count)];
        }
        /// <summary>
        /// Determines a client's preferred intensity level based on age, weight, and height.
        /// Simplified logic: Younger and fitter clients prefer higher intensity.
        /// </summary>
        public IntensityLevel GetClientIntensityLevel(DateTime dob, int weight, int height)
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
        public string GetRandomClientBio(string FirstName, ClientExperience experience, string Country)
        {
            var nameStringIntroductionArray = new[] { $"Hi, I'm {FirstName} from {Country}", $"Hello! My name is {FirstName} and I live in {Country}", $"Hey there, I'm {FirstName} from {Country}" };
            // Use bios from _clientData.ClientBios
            string experienceKey = experience.ToString();
            if (_clientData.ClientBios.TryGetValue(experienceKey, out var bios) && bios.Length > 0)
            {
                var intro = nameStringIntroductionArray[_random.Next(nameStringIntroductionArray.Length)];
                var bio = bios[_random.Next(bios.Length)];
                return $"{intro}{bio}";
            }
            else
            {
                // Fallback if no bios found for experience
                var intro = nameStringIntroductionArray[_random.Next(nameStringIntroductionArray.Length)];
                return $"{intro}. I'm passionate about fitness and excited to embark on this journey.";
            }

        }
        /// <summary>
        /// Generates a random trainer bio based on specialization.
        /// Combines a random adjective, specialization, and filler for variety.
        /// </summary>
        public string GetRandomTrainerBio(TrainerSpecialization specialization)
        {
            var adjectives = _trainerData.TrainerBio.Adjectives;
            var fillers = _trainerData.TrainerBio.Fillers;
            var templates = _trainerData.TrainerBio.Templates;

            var adj = adjectives[_random.Next(adjectives.Length)];
            var filler = fillers[_random.Next(fillers.Length)];
            var template = templates[_random.Next(templates.Length)];

            // Replace placeholders in template
            var bio = template
                .Replace("{adj}", adj)
                .Replace("{spec}", specialization.ToString().ToLower())
                .Replace("{filler}", filler);

            return bio;
        }
        /// <summary>
        /// Returns a random list of TrainerSpecialization values (1 to 3 specializations per trainer).
        /// </summary>
        public List<TrainerSpecialization> GetRandomTrainerSpecializations()
        {
            var specializations = Enum.GetValues(typeof(TrainerSpecialization)).Cast<TrainerSpecialization>().ToList();
            int count = _random.Next(1, 4); // Each trainer can have 1 to 3 specializations
            return specializations.OrderBy(x => _random.Next()).Take(count).ToList();
        }
        /// <summary>
        /// Returns a random last name from a predefined list.
        /// </summary>
        public string GetRandomLastName()
        {
            return _idInfoData.LastNames[_random.Next(_idInfoData.LastNames.Length)];
        }
        /// <summary>
        /// Returns a random Date of Birth.
        /// </summary>
        public DateTime GetRandomDateOfBirth()
        {
            int year = _random.Next(1950, 2005); // Age between ~18 and ~73
            int month = _random.Next(1, 13);
            int day = _random.Next(1, 28); // Simplified to avoid month length issues
            return new DateTime(year, month, day);
        }
        /// <summary>
        /// Returns a random country from a predefined list.
        /// </summary>
        public string GetRandomCountry()
        {
            var countries = _idInfoData.Countries;
            return countries[_random.Next(countries.Length)];
        }
        /// <summary>
        /// Returns a random city.
        /// </summary>
        public string GetRandomCity(string country)
        {
            if (_idInfoData.CitiesByCountry.TryGetValue(country, out var cities) && cities.Length > 0)
            {
                return cities[_random.Next(cities.Length)];
            }
            return "Unknown City";
        }
        /// <summary>
        /// Returns a random phone number.
        /// </summary>
        public string GenerateRandomPhoneNumber()
        {
            // Country-agnostic: E.164-like format
            var countryCode = _random.Next(1, 999); // Random country code
            var areaCode = _random.Next(100, 999);
            var subscriber = _random.Next(1000000, 9999999);
            return $"+{countryCode} {areaCode} {subscriber}";
        }
        /// <summary>
        /// Returns a random state if the country is USA.
        /// </summary>
        public string GetRandomState()
        {
            var states = _idInfoData.States;
            return states[_random.Next(states.Length)];
        }
        /// <summary>
        /// Returns a random street address.
        /// </summary>
        public string GetRandomStreetAddress(string country)
        {
            int streetNumber = _random.Next(1, 9999);
            if (_idInfoData.StreetNamesByCountry.TryGetValue(country, out var streetNames) && streetNames.Length > 0)
            {
                var streetName = streetNames[_random.Next(streetNames.Length)];
                return $"{streetNumber} {streetName}";
            }
            return $"{streetNumber} Fitness St";
        }
        /// <summary>
        /// Returns a random zip code.
        /// </summary>
        public string GetRandomZipCode()
        {
            return $"{_random.Next(10000, 99999)}";
        }

        #endregion

        #endregion

    }
}