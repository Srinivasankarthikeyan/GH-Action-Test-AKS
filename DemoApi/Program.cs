var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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

// Add a new welcome API endpoint for testing
app.MapGet("/welcome", () => "Welcome to the Demo API! V1")
   .WithName("WelcomeApi");

// VULNERABILITY TEST: Command injection vulnerability
app.MapGet("/execute", (string command) =>
{
    // Unsafe - directly executing user input
    var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
    {
        FileName = "/bin/sh",
        Arguments = $"-c {command}",
        RedirectStandardOutput = true,
        UseShellExecute = false
    });
    return "Executed";
})
.WithName("ExecuteCommand");

// VULNERABILITY TEST: Hardcoded secrets
var apiKey = "sk-1234567890abcdefghijklmnop"; // Hardcoded secret
var dbPassword = "AdminPassword123!"; // Hardcoded credentials

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
