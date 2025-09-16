using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record MeasurementDto(
        int Id,
        GoalUnit Unit,
        double Value,
        DateTime Date,
        IntensityLevel Intensity,
        bool IsPersonalBest,
        string? Notes,
        int ClientId,
        ClientDto Client
    );

    public record CreateMeasurementDto(
        GoalUnit Unit,
        double Value,
        DateTime Date,
        IntensityLevel Intensity,
        bool IsPersonalBest = false,
        string? Notes = null,
        int ClientId = 0
    );

    public record UpdateMeasurementDto(
        int Id,
        GoalUnit? Unit = null,
        double? Value = null,
        DateTime? Date = null,
        IntensityLevel? Intensity = null,
        bool? IsPersonalBest = null,
        string? Notes = null
    );
}