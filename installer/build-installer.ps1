param(
    [string]$Configuration = "Release",
    [switch]$SkipBuild,
    [switch]$SkipMsi
)

$ErrorActionPreference = "Stop"
$root = Resolve-Path "$PSScriptRoot\.."
$installerDir = $PSScriptRoot

if (-not $SkipBuild) {
    Write-Host "Building solution in $Configuration mode..." -ForegroundColor Cyan
    & dotnet publish "$root\src\ShoppingMall.Client\ShoppingMall.Client.csproj" -c $Configuration -f net8.0-windows --self-contained true -o "$root\src\ShoppingMall.Client\bin\$Configuration\net8.0-windows\publish"
    if (-not $?) { throw "Client publish failed" }

    & dotnet publish "$root\src\ShoppingMall.Server\ShoppingMall.Server.csproj" -c $Configuration -f net8.0 --self-contained true -o "$root\src\ShoppingMall.Server\bin\$Configuration\net8.0\publish"
    if (-not $?) { throw "Server publish failed" }

    & dotnet publish "$root\cloud\ShoppingMall.CloudDashboard\ShoppingMall.CloudDashboard.csproj" -c $Configuration -f net8.0 -o "$root\cloud\ShoppingMall.CloudDashboard\bin\$Configuration\net8.0\publish"
    if (-not $?) { throw "Cloud Dashboard publish failed" }
}

Write-Host "Creating ZIP distribution..." -ForegroundColor Cyan
$distDir = "$installerDir\dist"
if (Test-Path $distDir) { Remove-Item -Recurse -Force $distDir }
New-Item -ItemType Directory -Path $distDir -Force

# Copy client publish
Copy-Item "$root\src\ShoppingMall.Client\bin\$Configuration\net8.0-windows\publish\*" $distDir -Recurse -Force

# Copy server publish
$serverDist = "$distDir\Server"
New-Item -ItemType Directory -Path $serverDist -Force
Copy-Item "$root\src\ShoppingMall.Server\bin\$Configuration\net8.0\publish\*" $serverDist -Recurse -Force

# Copy cloud dashboard
$cloudDist = "$distDir\CloudDashboard"
New-Item -ItemType Directory -Path $cloudDist -Force
Copy-Item "$root\cloud\ShoppingMall.CloudDashboard\bin\$Configuration\net8.0\publish\*" $cloudDist -Recurse -Force

# Create run scripts
@"
@echo off
echo Starting Shopping Mall POS Client...
start "" "%~dp0ShoppingMall.Client.exe"
"@ | Set-Content "$distDir\Run-POS.bat"

@"
@echo off
echo Starting Shopping Mall Server (port 5000)...
start /B "" "%~dp0Server\ShoppingMall.Server.exe"
echo Server started. Press any key to stop.
pause
taskkill /f /im ShoppingMall.Server.exe 2>nul
"@ | Set-Content "$distDir\Run-Server.bat"

# Create config
@"
{
  "ServerUrl": "http://localhost:5000",
  "TerminalId": "00000000-0000-0000-0000-000000000000",
  "StoreId": "00000000-0000-0000-0000-000000000000",
  "TerminalName": "POS-1",
  "AutoConnect": true,
  "IsConfigured": false
}
"@ | Set-Content "$distDir\appsettings.json"

Write-Host "Creating ZIP archive..." -ForegroundColor Cyan
$zipPath = "$installerDir\ShoppingMall-POS-v1.0.0.zip"
if (Test-Path $zipPath) { Remove-Item -Force $zipPath }
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($distDir, $zipPath)

if (-not $SkipMsi) {
    Write-Host "Building MSI installer..." -ForegroundColor Cyan
    try {
        $wixTool = Get-Command wix -ErrorAction Stop
        # Generate component fragment from dist
        & "$installerDir\Generate-FilesFragment.ps1" *>$null
        & $wixTool.Source build "$installerDir\Product.wxs" -out "$installerDir\ShoppingMall-POS-v1.0.0.msi" -b "$distDir" -arch x64
        if ($LASTEXITCODE -eq 0) {
            Write-Host "MSI: $installerDir\ShoppingMall-POS-v1.0.0.msi" -ForegroundColor Yellow
            Write-Host "Size: $((Get-Item "$installerDir\ShoppingMall-POS-v1.0.0.msi").Length / 1MB) MB" -ForegroundColor Yellow
        } else {
            Write-Host "MSI build had warnings. ZIP is the primary distribution." -ForegroundColor DarkYellow
        }
    } catch {
        Write-Host "MSI build skipped (WiX Toolset not available): $_" -ForegroundColor DarkYellow
    }
}

Write-Host "Done!" -ForegroundColor Green
Write-Host "ZIP Distribution: $zipPath" -ForegroundColor Yellow
Write-Host "Size: $((Get-Item $zipPath).Length / 1MB) MB" -ForegroundColor Yellow

# Cleanup dist
Remove-Item -Recurse -Force $distDir
