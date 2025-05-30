using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog;
using WeatherService.Application.Services;
using WeatherService.Core.Interfaces;
using WeatherService.Infrastructure.Configuration;
using WeatherService.Infrastructure.Data;
using WeatherService.Infrastructure.Repositories;
using WeatherService.Infrastructure.Resilience;
using WeatherService.Infrastructure.Services;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Reduce noise from Microsoft logs
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console() // Console output for development
    .WriteTo.File(
        path: "logs/weather-service-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7) // Rolling logs, keep 7 days
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting WeatherService application");

builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IWeatherService, OpenMeteoWeatherService>();
builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddScoped<WeatherDataService>();
builder.Services.AddMemoryCache();

// Configure resilience options
builder.Services.Configure<ResilienceOptions>(
    builder.Configuration.GetSection(ResilienceOptions.SectionName));

// Register resilience services
builder.Services.AddScoped<IPolicyFactory, PolicyFactory>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
