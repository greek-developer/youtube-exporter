# YouTubeExporter

This application exposes YouTube channel and video statistics as Prometheus metrics on port 5051, at /metrics.

## Features
- Exposes /metrics endpoint for Prometheus scraping
- Reports total views, subscribers, likes count
- Reports per-video views and likes count, labeled as short/long
- Uses YouTube Data API (requires API key)

## Setup

1. **Obtain a YouTube Data API v3 key:**
  1. Go to the [Google Cloud Console](https://console.cloud.google.com/).
  2. Create a new project (or select an existing one).
  3. In the left menu, go to **APIs & Services > Library**.
  4. Search for **YouTube Data API v3** and click **Enable**.
  5. In the left menu, go to **APIs & Services > Credentials**.
  6. Click **Create Credentials** > **API key**.
  7. Copy the generated API key.
  8. (Optional) Click **Restrict Key** to limit usage for security.
2. Set the API key and channel ID in `appsettings.json` (to be created).
3. Run the app: `dotnet run --project YoutubeExporter`

## Configuration

Add `Channel Id` and `API Key`  to `appsettings.json`:
```json
{
  "YouTube": {
    "ApiKey": "YOUR_API_KEY",
    "ChannelId": "YOUR_CHANNEL_ID"
  }
}
```

Optionally edit the `urls` field to change the port

## Prometheus Metrics
- `youtube_total_views`
- `youtube_total_subscribers`
- `youtube_video_views{video_id,format}`
- `youtube_video_view_likes{video_id,format}`

## Notes
- "format" label is "short" if video duration < 60s, else "long".
- Metrics are updated periodically (default: every 5 minutes).

## Versioning

The package version lives in [`Directory.Build.props`](Directory.Build.props) at the repo root, not in any `.csproj`. Bump it with:

```powershell
./scripts/bump-version.ps1          # bump the minor version (x.Y.0)
./scripts/bump-version.ps1 --major  # bump the major version (X.0.0)
./scripts/bump-version.ps1 --push   # also push the bump commit
```

The script refuses to run with uncommitted changes, and commits the bump as `bump version to x.x.x`.
