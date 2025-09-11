using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Api.Dtos;

namespace Api.Models
{
    public class MedicalHistory : BaseEntity
    {
        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public ICollection<MedicalCondition> Conditions { get; set; } = new List<MedicalCondition>();

        [Required]
        public ICollection<MedicationType> MedicationTypes { get; set; } = new List<MedicationType>();

        [Required]
        public ICollection<SurgeryType> Surgeries { get; set; } = new List<SurgeryType>();

        public IntensityLevel? RecommendedIntensityLevel { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public Client Client { get; set; } = null!;


        public MedicalHistoryDto ToMedicalHistoryDto()
        {
            return new MedicalHistoryDto(
                Id,
                Description,
                Conditions,
                MedicationTypes,
                Surgeries,
                RecommendedIntensityLevel,
                ClientId,
                Client
            );
        }

        public CreateMedicalHistoryDto ToCreateMedicalHistoryDto()
        {
            return new CreateMedicalHistoryDto(
                Description,
                Conditions,
                MedicationTypes,
                Surgeries,
                RecommendedIntensityLevel,
                ClientId
            );
        }

        public UpdateMedicalHistoryDto ToUpdateMedicalHistoryDto()
        {
            return new UpdateMedicalHistoryDto(
                Id,
                Description,
                Conditions,
                MedicationTypes,
                Surgeries,
                RecommendedIntensityLevel
            );
        }
    }
}