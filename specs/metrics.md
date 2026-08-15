# Spec: YouTube metrics export

How the product exposes a YouTube channel's statistics as Prometheus metrics. Present tense
describes shipped behaviour; **`[not-yet-implemented]`** marks a planned target not yet met.

## Stats collection

A background service periodically queries the YouTube Data API for the configured channel and
its videos, and stores the result in an in-memory cache. The refresh runs on a fixed interval
and replaces the cached snapshot on each successful fetch. A failed fetch leaves the previous
snapshot in place.

The channel is identified by `YouTube:ChannelId`; access uses `YouTube:ApiKey`. Both are
supplied via configuration (secrets in `appsettings.User.json`).

## `GET /metrics`

Renders the cached stats in Prometheus text exposition format:

- `youtube_total_views` — channel lifetime view count.
- `youtube_total_subscribers` — channel subscriber count.
- Per-video series for views, and a flag distinguishing Shorts (duration < 60s) from
  long-form videos.

Before the first successful fetch, the endpoint returns a `# No data available yet` comment
with a `200` status, so a scraper sees a valid-but-empty response rather than an error.

## `GET /api/diagnostics/version`

Returns the contents of `ProductionVersion.json` — the version, the commit subject and full
SHA, and the build time — identifying exactly which build is running. Available without
authentication, like `/metrics`.

## Refresh interval configuration — `[not-yet-implemented]`

The refresh cadence is currently fixed in code; making it a configuration value is planned.
