using YoutubeExporter.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add appsettings.User.json after the other config files
builder.Configuration
    .AddJsonFile("appsettings.User.json", optional: true, reloadOnChange: true);

builder.Services
    .Configure<YouTubeStatsOptions>(builder.Configuration.GetSection("YouTube"))
    .AddSingleton<YouTubeStatsService>()
    .AddSingleton<StatsCache>()
    .AddHostedService<StatsCacheUpdater>()
    .AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
