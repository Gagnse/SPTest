using backend.Data;
using SpaceLogic.Data.Admin;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ➕ Configuration CORS (autorise React en local et dans Docker)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // ✅ Autorise les deux ports
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ➕ Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ➕ Enregistre AppDbContext pour la base admin
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AdminConnection")));

// Enregistre les contrôleurs MVC
builder.Services.AddControllers();

var app = builder.Build();

// ➕ Forcer le backend à écouter sur le port 5000 (nécessaire pour Docker)
app.Urls.Add("http://+:5000");

// ➕ Middleware CORS
app.UseCors();

// ➕ Swagger toujours activé (utile avec Docker)
app.UseSwagger();
app.UseSwaggerUI();

// 🔒 Redirection HTTPS désactivée pour l'instant (à réactiver plus tard)
// app.UseHttpsRedirection();

app.MapGet("/test-admin-db", async (AdminDbContext db) =>
{
    var userCount = await db.Users.CountAsync();
    return Results.Ok($"Il y a {userCount} utilisateur(s) dans la base admin.");
});

// ➕ Exemple d’API
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )
    ).ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// 🔎 Affiche la chaîne de connexion en console
Console.WriteLine("Connection string : " + builder.Configuration.GetConnectionString("DefaultConnection"));

// Active les routes des contrôleurs
app.MapControllers();

app.Run();

// ➕ Modèle pour l'exemple d'API
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
