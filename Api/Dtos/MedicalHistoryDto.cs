using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record MedicalHistoryDto(
        int Id,
        string? Description,
        ICollection<MedicalCondition> Conditions,
        ICollection<MedicationType> MedicationTypes,
        ICollection<SurgeryType> Surgeries,
        IntensityLevel? RecommendedIntensityLevel,
        int ClientId,
        ClientDto Client
    );

    public record CreateMedicalHistoryDto(
        string? Description,
        ICollection<MedicalCondition> Conditions,
        ICollection<MedicationType> MedicationTypes,
        ICollection<SurgeryType> Surgeries,
        IntensityLevel? RecommendedIntensityLevel,
        int ClientId
    );

    public record UpdateMedicalHistoryDto(
        int Id,
        string? Description = null,
        ICollection<MedicalCondition>? Conditions = null,
        ICollection<MedicationType>? MedicationTypes = null,
        ICollection<SurgeryType>? Surgeries = null,
        IntensityLevel? RecommendedIntensityLevel = null
    );
}