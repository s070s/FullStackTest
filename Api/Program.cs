using Microsoft.EntityFrameworkCore;
using Api.Data;

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


var app = builder.Build();

//Use CORS before MapGet/MapPost calls
app.UseCors("AllowSpecificOrigin");


//Apply any pending EF Core migrations at startup (dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => $"Hello User! {DateTime.Now}");
// Register user endpoints from UsersEndpoints
Api.Endpoints.UsersEndpoints.MapUserEndpoints(app);





app.Run();

