using Microsoft.EntityFrameworkCore;
using Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Api.Repositories.UnitOfWork;
using Api.Endpoints;


var builder = WebApplication.CreateBuilder(args);
#region Database Config
//Database Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=app.db";

//Register DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// Configure JSON serialization to handle enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
#endregion

#region CORS Configuration
//Register CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            // Prefer Cors:AllowedOrigins array from config; fallback to Frontend:Url (comma-separated)
            var configured = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            string[] origins;
            if (configured is not null && configured.Length > 0)
            {
                origins = configured;
            }
            else
            {
                var frontendConfig = builder.Configuration["Frontend:Url"] ?? "http://localhost:5173";
                origins = frontendConfig.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }

            policy.WithOrigins(origins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});
#endregion

#region JWT Authentication
// JWT Authentication configuration
var jwtKey = builder.Configuration["Jwt:Key"];// Use a secure key in production
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException("JWT configuration missing: Jwt:Key or Jwt:Issuer is not set.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
#endregion

#region Rate Limiting
//RateLimiting 
builder.Services.AddRateLimiter(options =>
{
    //for login endpoint to mitigate brute-force attacks
    options.AddFixedWindowLimiter("login", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;// 5 requests
        limiterOptions.Window = TimeSpan.FromMinutes(1);// per minute
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2;
    });
});
#endregion

#region Service Registration
// Register authorization services
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Trainer", policy => policy.RequireRole("Trainer"));
    options.AddPolicy("Client", policy => policy.RequireRole("Client"));
});
// Register JwtService
builder.Services.AddScoped<IJwtService, JwtService>();
// Register ValidationService
builder.Services.AddScoped<IValidationService, ValidationService>();
// Register DbContextLifeCycleService
builder.Services.AddScoped<IDbContextLifecycleService, DbContextLifecycleService>();
// Register PaginationServices(Sorting and Pagination)
builder.Services.AddScoped<IPaginationService, PaginationService>();
// Register DatabaseSeederService
builder.Services.AddScoped<DatabaseSeederService>();

#region Repositories
// Register Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
#endregion

#endregion

var app = builder.Build();

#region Seeding
// Seed the database with initial data. If seeding fails, the app will still start.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<DatabaseSeederService>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while Seeding the database");
        // Don't throw - let the app continue even if seeding fails
    }
}
#endregion


#region Enable Middleware
//Use CORS before MapGet/MapPost calls
app.UseCors("AllowFrontend");

// Enable authentication middleware
app.UseAuthentication();

// Enable authorization middleware
app.UseAuthorization();

// Enable rate limiting middleware
app.UseRateLimiter();
// Serve static files (e.g., profile images, uploads) from wwwroot or configured static files directory.
app.UseStaticFiles();
#endregion

// Uncomment the following block to automatically apply EF Core migrations at startup (useful for development only).
// In production, handle migrations separately to avoid data loss or downtime.
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

#region Endpoints
// Basic home endpoint
app.MapGet("/", () => $"This server is running and ready to accept requests");
// Register modular endpoint groups for better organization and separation of concerns.
AdminUserEndpoints.MapAdminUserEndpoints(app);
UserProfileEndpoints.MapUserProfileEndpoints(app);
AuthEndpoints.MapAuthEndpoints(app);
TrainerUserEndpoints.MapTrainerUserEndpoints(app);
#endregion

app.Run();

