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

Versions are computed by [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning)
from [`version.json`](version.json) plus the git height — there is no hardcoded version anywhere.
`version.json` holds the `major.minor`; the patch is the number of commits since that value last
changed, so **every commit bumps the patch automatically**.

Install the CLI once:

```powershell
dotnet tool install --global nbgv
```

### Viewing the version

```powershell
nbgv get-version                    # full summary for HEAD
nbgv get-version -v SimpleVersion   # just x.y.z, for scripts
nbgv get-version -f json            # everything, as JSON
```

### Setting the version

The patch bumps on its own with every commit. To change the major or minor, hand-edit the
`version` field in `version.json` and commit it — the patch count restarts from there:

```json
"version": "1.3"
```

Do **not** run `nbgv set-version`. It rewrites `version.json` from scratch and silently drops the
`publicReleaseRefSpec` and `cloudBuild` settings this repo relies on. Never add a `<Version>`
element to `Directory.Build.props` or a `.csproj` either — it would override the computed version.

### Releases

A build from `release/production` is a public release and gets a clean version (`1.3.4`). Every
other branch is a prerelease and gets a commit-id suffix (`1.3.4-g1a2b3c4`).
