// backend/Program.cs
using backend.Data;
using SpaceLogic.Data.Admin;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Collections;

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
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var adminConnection = builder.Configuration.GetConnectionString("AdminConnection");

// Try to build connection string from PostgreSQL environment variables if connection strings are empty
if (string.IsNullOrEmpty(adminConnection))
{
    var pgHost = Environment.GetEnvironmentVariable("PGHOST");
    var pgDatabase = Environment.GetEnvironmentVariable("PGDATABASE");
    var pgUser = Environment.GetEnvironmentVariable("PGUSER");
    var pgPassword = Environment.GetEnvironmentVariable("PGPASSWORD");
    var pgSslMode = Environment.GetEnvironmentVariable("PGSSLMODE") ?? "require";
    
    if (!string.IsNullOrEmpty(pgHost) && !string.IsNullOrEmpty(pgDatabase) && !string.IsNullOrEmpty(pgUser) && !string.IsNullOrEmpty(pgPassword))
    {
        adminConnection = $"Host={pgHost};Database={pgDatabase};Username={pgUser};Password={pgPassword};SSL Mode={pgSslMode};Trust Server Certificate=true";
        defaultConnection = adminConnection; // Use same for both
        Console.WriteLine("✅ Built connection string from PostgreSQL environment variables");
    }
}

Console.WriteLine("=== CONNECTION STRING DEBUG ===");
Console.WriteLine($"Default Connection: {defaultConnection ?? "NULL"}");
Console.WriteLine($"Admin Connection: {adminConnection ?? "NULL"}");

// Show PostgreSQL environment variables
Console.WriteLine("=== POSTGRESQL ENVIRONMENT VARIABLES ===");
Console.WriteLine($"PGHOST: {Environment.GetEnvironmentVariable("PGHOST") ?? "NULL"}");
Console.WriteLine($"PGDATABASE: {Environment.GetEnvironmentVariable("PGDATABASE") ?? "NULL"}");
Console.WriteLine($"PGUSER: {Environment.GetEnvironmentVariable("PGUSER") ?? "NULL"}");
Console.WriteLine($"PGPASSWORD: {(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PGPASSWORD")) ? "NULL" : "***SET***")}");
Console.WriteLine($"PGSSLMODE: {Environment.GetEnvironmentVariable("PGSSLMODE") ?? "NULL"}");

// Show all environment variables for debugging
Console.WriteLine("=== ENVIRONMENT VARIABLES ===");
foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
{
    if (env.Key.ToString().Contains("Connection") || env.Key.ToString().Contains("Jwt") || env.Key.ToString().StartsWith("PG"))
    {
        var value = env.Key.ToString().Contains("Password") ? "***HIDDEN***" : env.Value?.ToString();
        Console.WriteLine($"  {env.Key} = {value}");
    }
}

Console.WriteLine("=== ALL CONFIGURATION KEYS ===");
foreach (var item in builder.Configuration.AsEnumerable())
{
    if (item.Key.Contains("Connection") || item.Key.Contains("Jwt"))
    {
        Console.WriteLine($"  {item.Key} = {item.Value}");
    }
}

if (string.IsNullOrEmpty(defaultConnection) || string.IsNullOrEmpty(adminConnection))
{
    Console.WriteLine("ERROR: Connection strings are null or empty!");
    throw new InvalidOperationException("Database connection strings are not configured properly!");
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(defaultConnection, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });
});

builder.Services.AddDbContext<AdminDbContext>(options =>
{
    options.UseNpgsql(adminConnection, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });
});

// ➕ JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? builder.Configuration["Jwt__Key"] ?? "your-secret-key-here-make-it-long-and-secure-fallback-key-for-development";
Console.WriteLine($"JWT Key loaded: {(string.IsNullOrEmpty(jwtKey) ? "EMPTY" : "OK")}");
Console.WriteLine($"JWT Key length: {jwtKey?.Length ?? 0}");

if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 16)
{
    throw new InvalidOperationException("JWT Key must be at least 16 characters long. Please set Jwt__Key in your environment variables.");
}

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

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

// ➕ Authentication & Authorization (ordre important!)
app.UseAuthentication();
app.UseAuthorization();

// 🔒 Redirection HTTPS désactivée pour l'instant (à réactiver plus tard)
// app.UseHttpsRedirection();

app.MapGet("/test-admin-db", async (AdminDbContext db) =>
{
    try
    {
        Console.WriteLine("=== DATABASE CONNECTION TEST ===");
        Console.WriteLine("Testing database connection...");
        Console.WriteLine("Attempting to connect to Neon database...");
        
        // Get the actual connection string being used
        var connectionString = db.Database.GetConnectionString();
        Console.WriteLine($"Using connection string: {connectionString}");
        
        // Test basic connection with detailed error catching
        bool canConnect;
        try
        {
            canConnect = await db.Database.CanConnectAsync();
            Console.WriteLine($"CanConnectAsync result: {canConnect}");
        }
        catch (Exception connEx)
        {
            Console.WriteLine($"❌ CanConnectAsync threw exception: {connEx.Message}");
            Console.WriteLine($"Exception type: {connEx.GetType().Name}");
            Console.WriteLine($"Stack trace: {connEx.StackTrace}");
            if (connEx.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {connEx.InnerException.Message}");
            }
            return Results.Problem($"Connection test failed with exception: {connEx.Message}");
        }
        
        if (!canConnect)
        {
            Console.WriteLine("❌ CanConnectAsync returned false");
            
            // Try to create a direct connection to get more specific error
            try
            {
                using var connection = new Npgsql.NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                Console.WriteLine("✅ Direct NpgsqlConnection succeeded!");
                await connection.CloseAsync();
            }
            catch (Exception directEx)
            {
                Console.WriteLine($"❌ Direct connection failed: {directEx.Message}");
                Console.WriteLine($"Exception type: {directEx.GetType().Name}");
                if (directEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {directEx.InnerException.Message}");
                }
                return Results.Problem($"Direct connection failed: {directEx.Message}");
            }
            
            return Results.Problem("Cannot connect to the database - CanConnectAsync failed.");
        }
        
        Console.WriteLine("✅ Database connection successful!");
        
        // Try to query users table
        try
        {
            var userCount = await db.Users.CountAsync();
            Console.WriteLine($"✅ Successfully queried users table! User count: {userCount}");
            return Results.Ok($"✅ Database connected! Il y a {userCount} utilisateur(s) dans la base admin.");
        }
        catch (Exception tableEx)
        {
            Console.WriteLine($"❌ Failed to query users table: {tableEx.Message}");
            return Results.Problem($"Database connected but users table query failed: {tableEx.Message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Unexpected error: {ex.Message}");
        Console.WriteLine($"Exception type: {ex.GetType().Name}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
        return Results.Problem($"❌ Unexpected error: {ex.Message}");
    }
});

// ➕ Exemple d'API
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