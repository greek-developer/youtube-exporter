# Publishes a self-contained, single-file win-x64 build into ./release and zips it.
# The version is computed by Nerdbank.GitVersioning (version.json + git height) - never
# hardcoded here. Install the CLI once with: dotnet tool install --global nbgv

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repoRoot
try {
    if (Test-Path ./release) {
        Remove-Item ./release -Recurse -Force -Confirm:$false
    }

    # Ask NBGV for the computed version so the zip name matches the built assembly.
    $version = (nbgv get-version -v SimpleVersion).Trim()

    $publishArgs = @(
        "publish",
        "./src/YouTubeExporter/YouTubeExporter.csproj",
        "-c", "Release",
        "-f", "net8.0",
        "-r", "win-x64",
        "-p:SelfContained=true",
        "-p:PublishSingleFile=true",
        "-o", "./release/win-x64"
    )

    dotnet @publishArgs

    # Never ship the developer's local secrets file.
    Remove-Item ./release/win-x64/appsettings.User.json -Force -ErrorAction SilentlyContinue

    Compress-Archive -Path "./release/win-x64/" -DestinationPath "./release/YouTubeExporter-$version.windows-amd64.zip" -Force
}
finally {
    Pop-Location
}
