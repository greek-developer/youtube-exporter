# Bumps the <Version> in Directory.Build.props and commits the change.
# Usage:
#   ./scripts/bump-version.ps1            # bump minor version (x.Y.0)
#   ./scripts/bump-version.ps1 --major    # bump major version (X.0.0)
#   ./scripts/bump-version.ps1 --push     # also push the commit

$ErrorActionPreference = "Stop"

$Major = $args -contains "--major"
$Push = $args -contains "--push"

$repoRoot = Split-Path -Parent $PSScriptRoot

Push-Location $repoRoot
try {
    $status = git status --porcelain
    if ($status) {
        Write-Error "Cannot proceed with uncommited changes, commit and re-try"
        exit 1
    }

    $propsPath = Join-Path $repoRoot "Directory.Build.props"
    if (-not (Test-Path $propsPath)) {
        Write-Error "Directory.Build.props not found at $propsPath"
        exit 1
    }

    $content = Get-Content -Raw -Path $propsPath
    if ($content -notmatch "<Version>(\d+)\.(\d+)\.(\d+)</Version>") {
        Write-Error "Could not find a <Version>major.minor.patch</Version> element in $propsPath"
        exit 1
    }

    $majorNum = [int]$Matches[1]
    $minorNum = [int]$Matches[2]
    $patchNum = [int]$Matches[3]

    if ($Major) {
        $majorNum++
        $minorNum = 0
        $patchNum = 0
    }
    else {
        $minorNum++
        $patchNum = 0
    }

    $newVersion = "$majorNum.$minorNum.$patchNum"
    $newContent = $content -replace "<Version>\d+\.\d+\.\d+</Version>", "<Version>$newVersion</Version>"
    Set-Content -Path $propsPath -NoNewline -Value $newContent

    Write-Host "Bumped version to $newVersion"

    git add $propsPath
    git commit -m "bump version to $newVersion"

    if ($Push) {
        git push
    }
}
finally {
    Pop-Location
}
