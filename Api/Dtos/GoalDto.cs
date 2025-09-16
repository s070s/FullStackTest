using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record GoalDto(
        int Id,
        GoalType GoalType,
        string Description,
        DateTime TargetDate,
        GoalStatus Status,
        int? GoalQuantity,
        GoalUnit? GoalUnit,
        int ClientId,
        ClientDto Client
    );

    public record CreateGoalDto(
        GoalType GoalType,
        string Description,
        DateTime TargetDate,
        int ClientId,
        GoalStatus Status,
        int? GoalQuantity = null,
        GoalUnit? GoalUnit = null
    );

    public record UpdateGoalDto(
        int Id,
        GoalType? GoalType = null,
        string? Description = null,
        DateTime? TargetDate = null,
        GoalStatus? Status = null,
        int? GoalQuantity = null,
        GoalUnit? GoalUnit = null
    );
}