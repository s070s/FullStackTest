using Api.Repositories.UnitOfWork;
using Api.Services;


namespace Api.Endpoints
{
    public static class ClientUserEndpoints
    {
        public static void MapClientUserEndpoints(this WebApplication app)
        {

            #region Client Subscription to Trainer
            //Allows a client to subscribe to trainers that they like
            app.MapPost("/clients/{userId:int}/subscribe/{trainerId:int}", async (
                int userId,
                int trainerId,
                HttpContext context,
                IUnitOfWork unitOfWork,
                IValidationService validator) =>
            {
                if (!validator.IsUserIdClaimMatchingRouteId(context, userId))
                    return Results.Unauthorized();
                //Get client by userId
                var client = await unitOfWork.Clients.GetClientByUserIdAsync(userId);
                if (client == null)
                    return Results.NotFound();
                var success = await unitOfWork.Clients.SubscribeToTrainerAsync(client.Id, trainerId);
                if (!success)
                    return Results.BadRequest("Subscription failed. Please check if the trainer exists or if you are already subscribed.");
                return Results.Ok(new { message = "Subscribed to trainer successfully." });

            }).RequireAuthorization("Client");
            #endregion



            #region Client Unsubscribe from Trainer
            //Allows a client to unsubscribe from trainers
            app.MapPost("/clients/{userId:int}/unsubscribe/{trainerId:int}", async (
                int userId,
                int trainerId,
                HttpContext context,
                IUnitOfWork unitOfWork,
                IValidationService validator) =>
            {
                if (!validator.IsUserIdClaimMatchingRouteId(context, userId))
                    return Results.Unauthorized();
                var client = await unitOfWork.Clients.GetClientByUserIdAsync(userId);
                if (client == null)
                    return Results.NotFound();
                var success = await unitOfWork.Clients.UnsubscribeFromTrainerAsync(client.Id, trainerId);
                if (!success)
                    return Results.BadRequest("Unsubscribe failed. Please check if you are subscribed.");
                return Results.Ok(new { message = "Unsubscribed from trainer successfully." });

            }).RequireAuthorization("Client");
            #endregion



            #region Get Subscribed Trainers
            //Returns list of trainer IDs that the client is subscribed to
            app.MapGet("/clients/{userId:int}/subscriptions", async (
                int userId,
                HttpContext context,
                IUnitOfWork unitOfWork,
                IValidationService validator) =>
            {
                if (!validator.IsUserIdClaimMatchingRouteId(context, userId))
                    return Results.Unauthorized();
                var client = await unitOfWork.Clients.GetClientByUserIdAsync(userId);
                if (client == null)
                    return Results.NotFound();
                var trainerIds = client.Trainers.Select(t => t.Id).ToList();
                return Results.Ok(trainerIds);
            }).RequireAuthorization("Client");
            #endregion
        }
    }
}