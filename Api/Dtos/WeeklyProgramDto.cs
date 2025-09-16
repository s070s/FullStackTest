using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record WeeklyProgramDto(
        int Id,
        string Name,
        string? Description,
        int DurationInWeeks,
        int CurrentWeek,
        bool IsCompleted,
        ICollection<WorkoutDto> Workouts,
        int ClientId,
        ClientDto Client
    );

    public record CreateWeeklyProgramDto(
        string Name,
        int DurationInWeeks,
        int ClientId,
        string? Description = null,
        ICollection<int>? WorkoutIds = null
    );

    public record UpdateWeeklyProgramDto(
        int Id,
        string? Name = null,
        string? Description = null,
        int? DurationInWeeks = null,
        int? CurrentWeek = null,
        ICollection<int>? WorkoutIds = null
    );
}