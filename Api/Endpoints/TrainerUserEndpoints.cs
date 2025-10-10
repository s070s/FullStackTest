using Api.Models;
using Api.Models.Enums;
using Api.Dtos;
using Api.Services;
using Api.Repositories.UnitOfWork;


namespace Api.Endpoints
{
    public static class TrainerUserEndpoints
    {
        public static void MapTrainerUserEndpoints(this WebApplication app)
        {
            #region Read All Trainers Paginated
            app.MapGet("/trainers", async (
                IUnitOfWork unitOfWork,
                IPaginationService paginationService,
                int? page,
                int? pageSize,
                string? sortBy,
                string? sortOrder
            ) =>
            {
                var (trainers, total) = await unitOfWork.Trainers.GetTrainersPagedAsync(paginationService, page, pageSize, sortBy, sortOrder);
                return Results.Ok(new { trainers, total });
            }).RequireAuthorization();
            #endregion

            #region Read Trainer By Id
            app.MapGet("/trainers/{id:int}", async (
                int id,
                IUnitOfWork unitOfWork
            ) =>
            {
                var trainer = await unitOfWork.Trainers.GetTrainerByIdAsync(id);
                if (trainer == null)
                { return Results.NotFound("Trainer not found."); }
                else
                {
                    return Results.Ok(trainer);
                }
            }).RequireAuthorization();
            #endregion
        }
    }
}