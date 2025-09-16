using Microsoft.EntityFrameworkCore;
using Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

//Database Connection String
var connectionString = builder.Configuration.GetConnectionString("Default") 
    ?? "Data Source=app.db";

//Register DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

//Register CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Your frontend URL
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

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

var app = builder.Build();

//Use CORS before MapGet/MapPost calls

app.UseCors("AllowSpecificOrigin");

// Enable authentication middleware
app.UseAuthentication();

// Enable authorization middleware
app.UseAuthorization();

// Enable rate limiting middleware
app.UseRateLimiter();


//Apply any pending EF Core migrations at startup (dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => $"This server is running and ready to accept requests");
// Register user endpoints from UsersEndpoints
Api.Endpoints.UsersEndpoints.MapUserEndpoints(app);





app.Run();

