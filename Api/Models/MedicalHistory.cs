using Api.Models.BaseClasses;
using Api.Models.Enums;

namespace Api.Models
{
    public class MedicalHistory : BaseEntity
    {
        public string? Description { get; set; }
        public ICollection<MedicalCondition> Conditions { get; set; } = new List<MedicalCondition>();
        public ICollection<MedicationType> MedicationTypes { get; set; } = new List<MedicationType>();
        public ICollection<SurgeryType> Surgeries { get; set; } = new List<SurgeryType>();
        public IntensityLevel? RecommendedIntensityLevel { get; set; }
        // Foreign key to Client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
    }
}