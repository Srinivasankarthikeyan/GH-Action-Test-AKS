using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool",
    "Mild", "Warm", "Balmy", "Hot",
    "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/welcome", () => "Welcome to the Demo API! V1")
   .WithName("WelcomeApi");


// ====================================================
// Intentional vulnerabilities for CodeQL testing
// ====================================================

// SQL Injection
app.MapGet("/users", (string name) =>
{
    string query =
        "SELECT * FROM Users WHERE Name = '" + name + "'";

    return Results.Ok(query);
});

// Command Injection
app.MapGet("/ping", (string host) =>
{
    Process.Start("ping", host);
    return Results.Ok($"Pinging {host}");
});

// Path Traversal
app.MapGet("/file", (string filename) =>
{
    var content = File.ReadAllText("/tmp/" + filename);
    return content;
});

// Weak Encryption
app.MapGet("/encrypt", () =>
{
    using var des = DES.Create();
    return "Weak Encryption Enabled";
});

app.Run();

record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}