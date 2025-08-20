if (Test-Path ./release) {
    Remove-Item ./release -Recurse -Force -Confirm:$false
}

$version = "0.1.0"
$informationalVersion = "0.1.0-pre-release"

$publishArgs = @(
    "publish", 
    "./src/YouTubeExporter/YouTubeExporter.csproj",
    "-c", "Release",
    "-f", "net8.0",
    "-r", "win-x64",
    "-p:SelfContained=true",
    "-p:PublishSingleFile=true",
    "-p:Version=$version",
    "-p:FileVersion=$version",
    "-p:InformationalVersion=$informationalVersion",
    "-o", "./release/win-x64"
)

dotnet @publishArgs

Remove-Item ./release/win-x64/appsettings.User.json -Force

Compress-Archive -Path "./release/win-x64/" -DestinationPath "./release/YouTubeExporter-$($version).windows-amd64.zip" -Force