using Lycoris.Nacos.Extensions;
using Microsoft.AspNetCore.Mvc;
using Nacos.V2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 
builder.Services.AddNacosNamingConfiguration(opt =>
{
    opt.Server = new List<string>() { "your nacos service ipaddress" };
    opt.Namespace = "a2ca805b-e024-4230-854e-978ba7853a4b";
    opt.UserName = "your username";
    opt.Password = "your password";
    opt.ConfigUseRpc = true;
    opt.NamingUseRpc = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async ([FromServices] INacosNamingService _nameSvc) =>
{

    var listView = await _nameSvc.GetServicesOfServer(1, 20, "Galaxy").ConfigureAwait(false);

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}