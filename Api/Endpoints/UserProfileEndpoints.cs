using Api.Services;
using Api.Repositories.UnitOfWork;
using Api.Dtos;
using Api.Models;
using Api.Models.Enums;

namespace Api.Endpoints
{
    public static class UserProfileEndpoints
    {
        public static void MapUserProfileEndpoints(this WebApplication app)
        {

            #region Get Current Logged User Profile
            app.MapGet("/users/me", async (
                HttpContext context,
                IUnitOfWork unitOfWork
            ) =>
            {
                var userIdClaim = context.User.FindFirst("userid")?.Value;
                if (userIdClaim == null) return Results.Unauthorized();

                if (!int.TryParse(userIdClaim, out int userId))
                    return Results.Unauthorized();

                var userDto = await unitOfWork.Users.GetUserWithProfilesByIdAsync(userId);
                if (userDto == null) return Results.NotFound("User not found.");
                return Results.Ok(userDto);
            }).RequireAuthorization();
            #endregion

            #region Handle Photo Upload
            app.MapPost("/users/{id:int}/upload-photo", async (
                int id,
                HttpContext context,
                IUnitOfWork unitOfWork,
                IValidationService validator,
                IWebHostEnvironment env
            ) =>
            {
                var userIdClaim = context.User.FindFirst("userid")?.Value;
                if (userIdClaim == null || int.Parse(userIdClaim) != id)
                    return Results.Forbid();

                if (!context.Request.HasFormContentType)
                    return Results.BadRequest("Invalid form data.");

                var form = await context.Request.ReadFormAsync();
                var file = form.Files.GetFile("photo");
                if (file is null || file.Length == 0)
                    return Results.BadRequest("No file uploaded.");

                if (!validator.IsValidImage(file))
                    return Results.BadRequest("Invalid image file. Only JPEG and PNG formats under 2MB are allowed.");

                var (success, message, photoUrl) = await unitOfWork.Users.UploadUserProfilePhotoAsync(id, file, env);
                if (!success)
                {
                    if (message == "User not found.")
                        return Results.NotFound(message);
                    return Results.BadRequest(message);
                }
                return Results.Ok(new { message = "Profile photo uploaded successfully.", photoUrl });
            }).RequireAuthorization();
            #endregion

            #region Update User Profile
                        app.MapPut("/users/{id:int}/profile", async (
                int id,
                HttpContext context,
                IUnitOfWork unitOfWork,
                IValidationService validator
            ) =>
            {
                var userIdClaim = context.User.FindFirst("userid")?.Value;
                if (userIdClaim == null || int.Parse(userIdClaim) != id)
                    return Results.Forbid();
            
                var userDto = await unitOfWork.Users.GetUserWithProfilesByIdAsync(id);
                if (userDto == null) return Results.NotFound("User not found.");
            
                if (userDto.Role == UserRole.Client && userDto.ClientProfile != null)
                {
                    var updateData = await context.Request.ReadFromJsonAsync<UpdateClientProfileDto>();
                    if (updateData == null)
                        return Results.BadRequest("Invalid request data.");
            
                    var result = await unitOfWork.Clients.UpdateClientProfileAsync(id, updateData);
                    if (result == null)
                        return Results.NotFound("Client profile not found.");
                }
                else if (userDto.Role == UserRole.Trainer && userDto.TrainerProfile != null)
                {
                    var updateData = await context.Request.ReadFromJsonAsync<UpdateTrainerProfileDto>();
                    if (updateData == null)
                        return Results.BadRequest("Invalid request data.");
            
                    var result = await unitOfWork.Trainers.UpdateTrainerProfileAsync(id, updateData);
                    if (result == null)
                        return Results.NotFound("Trainer profile not found.");
                }
                else
                {
                    return Results.BadRequest("Invalid user role or profile.");
                }
            
                await unitOfWork.SaveChangesAsync();
                return Results.Ok(userDto);
            });
            #endregion
        }

    }


}