using Api.Models;
using Api.Models.Enums;

namespace Api.Dtos
{
    public record EquipmentDto(
        int Id,
        string Name,
        string? Description,
        ICollection<ExerciseDefinitionDto> Exercises
    );

    public record CreateEquipmentDto(
        string Name,
        string? Description = null,
        ICollection<int>? ExerciseIds = null
    );

    public record UpdateEquipmentDto(
        int Id,
        string? Name = null,
        string? Description = null,
        ICollection<int>? ExerciseIds = null
    );
}