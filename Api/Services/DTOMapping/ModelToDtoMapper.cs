using Api.Models;
using Api.Dtos;
using Api.Models.Enums;

namespace Api.Services.Mapping
{

    #region Interface
    /// <summary>
    /// Defines methods for mapping domain models to Data Transfer Objects (DTOs).
    /// </summary>
    public interface IModelToDtoMapper
    {
        ClientDto? ToClientDto(Client? src);
        TrainerDto? ToTrainerDto(Trainer? src);
    }
    #endregion

    #region Implementation

    ///<inheritdoc/>
    public class ModelToDtoMapper : IModelToDtoMapper
    {
        public ClientDto? ToClientDto(Client? src)
        {
            if (src == null) return null;

            return new ClientDto
            {
                Id = src.Id,
                UserId = src.UserId,
                Bio = src.Bio,
                ExperienceLevel = src.ExperienceLevel,
                MedicalHistory = src.MedicalHistory != null ? new MedicalHistoryDto() : null,
                PreferredIntensityLevel = src.PreferredIntensityLevel,
                FirstName = src.FirstName,
                LastName = src.LastName,
                DateOfBirth = src.DateOfBirth,
                PhoneNumber = src.PhoneNumber,
                Address = src.Address,
                City = src.City,
                State = src.State,
                ZipCode = src.ZipCode,
                Country = src.Country,
                Weight = src.Weight,
                Height = src.Height,
                BMR = src.BMR,
                BMI = src.BMI,
                Age = src.Age,
                Goals = src.Goals?.Select(g => new GoalDto()).ToList() ?? new List<GoalDto>(),
                Workouts = src.Workouts?.Select(w => new WorkoutDto()).ToList() ?? new List<WorkoutDto>(),
                Trainers = src.Trainers?.Select(t => new TrainerDto()).ToList() ?? new List<TrainerDto>(),
                Measurements = src.Measurements?.Select(m => new MeasurementDto()).ToList() ?? new List<MeasurementDto>(),
                ProfilePhotoUrl = src.ProfilePhotoUrl
            };
        }

        public TrainerDto? ToTrainerDto(Trainer? src)
        {
            if (src == null) return null;

            return new TrainerDto
            {
                Id = src.Id,
                UserId = src.UserId,
                Bio = src.Bio,
                FirstName = src.FirstName,
                LastName = src.LastName,
                DateOfBirth = src.DateOfBirth,
                PhoneNumber = src.PhoneNumber,
                Address = src.Address,
                City = src.City,
                State = src.State,
                ZipCode = src.ZipCode,
                Country = src.Country,
                Weight = src.Weight,
                Height = src.Height,
                BMR = src.BMR,
                BMI = src.BMI,
                Age = src.Age,
                Clients = src.Clients?.Select(c => new ClientDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    FirstName = c.FirstName,
                    LastName = c.LastName
                }).ToList() ?? new List<ClientDto>(),
                Specializations = src.Specializations?.ToList() ?? new List<TrainerSpecialization>(),
                Workouts = src.Workouts?.Select(w => new WorkoutDto
                {
                    Id = w.Id
                }).ToList() ?? new List<WorkoutDto>(),
                ProfilePhotoUrl = src.ProfilePhotoUrl
            };
        }
    }
    #endregion



}