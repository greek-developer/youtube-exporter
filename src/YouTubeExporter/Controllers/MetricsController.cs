using Microsoft.AspNetCore.Mvc;
using YoutubeExporter.Services;
using System.Text;

namespace YoutubeExporter.Controllers
{
    [ApiController]
    [Route("/metrics")]
    public class MetricsController : ControllerBase
    {
        private readonly StatsCache _cache;

        public MetricsController(StatsCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public IActionResult GetMetrics()
        {
            var stats = _cache.GetCachedStats();
            if (stats == null)
                return Content("# No data available yet\n", "text/plain; version=0.0.4");

            var (channel, videos) = stats.Value;
            var sb = new StringBuilder();

            if (channel?.Statistics != null)
            {
                sb.AppendLine($"youtube_total_views {channel.Statistics.ViewCount}");
                sb.AppendLine($"youtube_total_subscribers {channel.Statistics.SubscriberCount}");
            }

            foreach (var video in videos)
            {
                var isShort = false;
                if (video.ContentDetails?.Duration != null)
                {
                    var duration = System.Xml.XmlConvert.ToTimeSpan(video.ContentDetails.Duration);
                    isShort = duration.TotalSeconds < 60;
                }
                var format = isShort ? "short" : "long";
                var views = video.Statistics?.ViewCount ?? 0;
                var likes = video.Statistics?.LikeCount ?? 0;
                var safeTitle = video.Snippet?.Title?.Replace("\"", "'") ?? "unknown";
                var videoId = video.Id ?? "unknown";
                sb.AppendLine($"youtube_video_views{{video_id=\"{videoId}\",video_title=\"{safeTitle}\",format=\"{format}\"}} {views}");
                sb.AppendLine($"youtube_video_likes{{video_id=\"{videoId}\",video_title=\"{safeTitle}\",format=\"{format}\"}} {likes}");
            }

            return Content(sb.ToString().Replace("\r", ""), "text/plain; version=0.0.4");
        }
    }
}
