using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeExporter.Services
{
	public class YouTubeStatsOptions
	{
		public string ApiKey { get; set; } = string.Empty;
		public string ChannelId { get; set; } = string.Empty;
	}

	public class YouTubeStatsService
	{
		private readonly YouTubeService _youtubeService;
		private readonly string _channelId;

		public YouTubeStatsService(IOptions<YouTubeStatsOptions> options)
		{
			_youtubeService = new YouTubeService(new BaseClientService.Initializer
			{
				ApiKey = options.Value.ApiKey
			});
			_channelId = options.Value.ChannelId;
		}

		public async Task<(Channel? channel, IList<Video> videos)> GetStatsAsync()
		{
			var channelRequest = _youtubeService.Channels.List("statistics,contentDetails");
			channelRequest.Id = _channelId;
			var channelResponse = await channelRequest.ExecuteAsync();
			var channel = channelResponse.Items.FirstOrDefault();

			var uploadsPlaylistId = channel?.ContentDetails?.RelatedPlaylists?.Uploads;
			if (uploadsPlaylistId == null)
				return (channel, new List<Video>());

			var videos = new List<Video>();
			string? nextPageToken = null;
			do
			{
				var playlistRequest = _youtubeService.PlaylistItems.List("contentDetails");
				playlistRequest.PlaylistId = uploadsPlaylistId;
				playlistRequest.MaxResults = 50;
				playlistRequest.PageToken = nextPageToken;
				var playlistResponse = await playlistRequest.ExecuteAsync();
				var videoIds = playlistResponse.Items.Select(i => i.ContentDetails.VideoId).ToList();
				if (videoIds.Count > 0)
				{
					var videoRequest = _youtubeService.Videos.List("statistics,snippet,contentDetails");
					videoRequest.Id = string.Join(",", videoIds);
					var videoResponse = await videoRequest.ExecuteAsync();
					videos.AddRange(videoResponse.Items);
				}
				nextPageToken = playlistResponse.NextPageToken;
			} while (nextPageToken != null);

			return (channel, videos);
		}
	}
}
