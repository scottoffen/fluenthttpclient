#!/usr/bin/env pwsh
# test-coverage.ps1

$ErrorActionPreference = "Stop"

# Run tests and collect coverage
dotnet test src --collect:"XPlat Code Coverage" --settings src/coverlet.runsettings

# Find the most recent coverage file
$coverageFile = Get-ChildItem -Recurse -Filter "coverage.cobertura.xml" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if (-not $coverageFile) {
    Write-Error "❌ No coverage file found."
    exit 1
}

# Restore the report generator tool
dotnet tool restore --tool-manifest ./src/.config/dotnet-tools.json

# Generate the report
Push-Location ./src

dotnet tool run reportgenerator `
  -reports:$coverageFile.FullName `
  -targetdir:"../coverage" `
  -reporttypes:"HtmlInline_AzurePipelines;TextSummary"

Pop-Location

# Open report in default browser
$indexPath = Join-Path "coverage" "index.html"

if ($IsWindows) {
    Start-Process $indexPath
} elseif ($IsMacOS) {
    open $indexPath
} elseif ($IsLinux) {
    xdg-open $indexPath
} else {
    Write-Warning "Platform not detected - open coverage/index.html manually"
}
