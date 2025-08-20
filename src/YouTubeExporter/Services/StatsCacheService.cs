using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;

namespace YoutubeExporter.Services
{
	public class StatsCache
	{
		private (Channel? channel, IList<Video> videos)? _cache;
		private readonly object _lock = new();

		public (Channel? channel, IList<Video> videos)? GetCachedStats()
		{
			lock (_lock)
			{
				return _cache;
			}
		}

		public void SetCachedStats((Channel? channel, IList<Video> videos) stats)
		{
			lock (_lock)
			{
				_cache = stats;
			}
		}
	}
}
