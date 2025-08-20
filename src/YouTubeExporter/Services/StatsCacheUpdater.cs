namespace YoutubeExporter.Services
{
    public class StatsCacheUpdater : BackgroundService
    {
        private readonly YouTubeStatsService _statsService;
        private readonly StatsCache _cache;
        private readonly ILogger<StatsCacheUpdater> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public StatsCacheUpdater(YouTubeStatsService statsService, StatsCache cache, ILogger<StatsCacheUpdater> logger)
        {
            _statsService = statsService;
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var stats = await _statsService.GetStatsAsync();
                    _cache.SetCachedStats(stats);
                    _logger.LogInformation("YouTube stats updated at {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update YouTube stats");
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
