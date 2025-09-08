namespace Api.Dtos
{
        public record WorkoutDto(
        int Id,
        int ClientId,
        int TrainerId,
        DateTime Date,
        string Type,
        int DurationInMinutes,
        string Notes
    );
    public record CreateWorkoutDto(
        int ClientId,
        int TrainerId,
        DateTime Date,
        string Type,
        int DurationInMinutes,
        string Notes
    );

    public record UpdateWorkoutDto(
        DateTime? Date,
        string? Type,
        int? DurationInMinutes,
        string? Notes
    );


}