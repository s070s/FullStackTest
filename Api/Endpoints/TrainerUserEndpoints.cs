using Api.Repositories.UnitOfWork;


namespace Api.Endpoints
{
    public static class TrainerUserEndpoints
    {
        public static void MapTrainerUserEndpoints(this WebApplication app)
        {
            #region Read All Trainers Paginated
            //Returns  sorted and paged list of all trainers along with total count
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
        }
    }
}