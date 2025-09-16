using Api.Models.Enums;
using Api.Models;

namespace Api.Dtos
{
    public record WorkoutDto(
        int Id,
        ICollection<ClientDto> Clients,
        int? TrainerId,
        TrainerDto? Trainer,
        int? WeeklyProgramId,
        WeeklyProgramDto? WeeklyProgram,
        DateTime ScheduledDateTime,
        string? Type,
        int DurationInMinutes,
        string? Notes
    );

    public record CreateWorkoutDto(
        ICollection<int> ClientIds,
        int? TrainerId,
        int? WeeklyProgramId,
        DateTime ScheduledDateTime,
        string? Type,
        int DurationInMinutes,
        string? Notes = null
    );

    public record UpdateWorkoutDto(
        ICollection<int>? ClientIds = null,
        int? TrainerId = null,
        int? WeeklyProgramId = null,
        DateTime? ScheduledDateTime = null,
        string? Type = null,
        int? DurationInMinutes = null,
        string? Notes = null
    );
}