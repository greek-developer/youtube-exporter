# BRIEF.md — youtube-exporter

Project-specific ground truth. The general conventions live in [AGENTS.md](AGENTS.md) (synced
from the grdev standard, never edited here); everything that makes *this* project different
lives below.

## Overview

`youtube-exporter` is an **ASP.NET Core Web API** that publishes a YouTube channel's
statistics as **Prometheus metrics**. A background service periodically fetches channel and
video stats from the YouTube Data API, caches them, and the `/metrics` endpoint renders the
cache in Prometheus text format for a Prometheus server to scrape.

Not to be confused with [`youtube-cli`](https://github.com/greek-developer/youtube-cli), a
separate tool for *uploading* videos.

## Build & run

```powershell
dotnet build .\youtube-exporter.slnx
dotnet run --project .\src\YouTubeExporter\YouTubeExporter.csproj
```

Then:

| Endpoint | Returns |
|---|---|
| `GET /metrics` | Channel/video stats in Prometheus text format |
| `GET /api/diagnostics/version` | The running build's identity (from `ProductionVersion.json`) |

There is no test suite yet.

### Configuration

Settings live under the `YouTube` section. The API key and channel id are **secrets** and go
in `appsettings.User.json` (git-ignored) — never in `appsettings.json`, which holds only
placeholders:

```json
{
  "YouTube": { "ApiKey": "...", "ChannelId": "..." }
}
```

The listen URL defaults to `http://*:5051` (`urls` in `appsettings.json`).

### Packaging

`scripts/build.ps1` publishes a self-contained single-file win-x64 build into `./release` and
zips it. The version is computed by Nerdbank.GitVersioning — the script does **not** pass a
hardcoded `-p:Version`.

## Layout

Standard grdev layout. Notable project files under `src/YouTubeExporter`:

| Path | Holds |
|---|---|
| `Controllers/MetricsController.cs` | The `/metrics` Prometheus endpoint |
| `Controllers/DiagnosticsController.cs` | The `/api/diagnostics/version` endpoint |
| `Services/YouTubeStatsService.cs` | Fetches stats from the YouTube Data API |
| `Services/StatsCacheService.cs` | In-memory cache (`StatsCache`) of the latest stats |
| `Services/StatsCacheUpdater.cs` | Hosted service that refreshes the cache periodically |

## Stack

| Choice | Detail |
|---|---|
| Runtime | .NET 8, ASP.NET Core (`Microsoft.NET.Sdk.Web`) |
| YouTube access | `Google.Apis.YouTube.v3` |
| Metrics format | Prometheus text exposition (hand-rendered in `MetricsController`) |

## Never

- **Never put the YouTube API key or channel id in `appsettings.json`.** They are secrets —
  they belong in the git-ignored `appsettings.User.json`.
- **Never hardcode a version** in `build.ps1` or the `.csproj` — it would override the
  Nerdbank.GitVersioning computation.

## Decisions

- **2026-08-14** — Adopted the grdev standard. It is a Web API (not a CLI), so it keeps its
  name (no `-cli` suffix) and exposes its version at `GET /api/diagnostics/version`.
- **2026-08-14** — A build-time MSBuild target writes `ProductionVersion.json` into the
  content root; the diagnostics endpoint serves its contents.
- **2026-08-14** — `build.ps1` moved to `scripts/` and rewritten to take the version from
  `nbgv` instead of a hardcoded string.
- **2026-08-14** — Replaced the empty `GreekDev.YouTubeExporter.sln` (it referenced no
  projects) with `youtube-exporter.slnx`.
