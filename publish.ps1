# publish.ps1 - Builds both projects into publish_forms and compiles the standalone Installer EXE
# Output: Output\EODServiceManager_Setup.exe (Self-contained, ready for multi-PC distribution)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  Building & Packaging EOD Service Manager" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

Write-Host "`n[1/3] Cleaning publish_forms..." -ForegroundColor Cyan
if (Test-Path "$root\publish_forms") {
    Remove-Item "$root\publish_forms" -Recurse -Force
}

Write-Host "`n[2/3] Publishing self-contained Release binaries..." -ForegroundColor Cyan
Write-Host "  -> Publishing EODService engine (win-x64)..." -ForegroundColor Gray
dotnet publish "$root\EODService\EODService.csproj" `
    -c Release -r win-x64 --self-contained `
    -o "$root\publish_forms"
if ($LASTEXITCODE -ne 0) { Write-Error "EODService publish failed."; exit 1 }

Write-Host "  -> Publishing EODSettingsApp UI (win-x64)..." -ForegroundColor Gray
dotnet publish "$root\EODSettingsApp\EODSettingsApp.csproj" `
    -c Release -r win-x64 --self-contained `
    -o "$root\publish_forms"
if ($LASTEXITCODE -ne 0) { Write-Error "EODSettingsApp publish failed."; exit 1 }

Write-Host "`n[3/3] Compiling Inno Setup Installer..." -ForegroundColor Cyan
$isccPaths = @(
    "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe",
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    "C:\Program Files\Inno Setup 6\ISCC.exe"
)

$iscc = $null
foreach ($path in $isccPaths) {
    if (Test-Path $path) {
        $iscc = $path
        break
    }
}

if (-not $iscc) {
    $cmd = Get-Command iscc -ErrorAction SilentlyContinue
    if ($cmd) { $iscc = $cmd.Source }
}

if ($iscc) {
    Write-Host "  -> Using Inno Setup Compiler: $iscc" -ForegroundColor Gray
    if (-not (Test-Path "$root\Output")) {
        New-Item -ItemType Directory -Path "$root\Output" | Out-Null
    }
    & "$iscc" "$root\EODServiceInstaller.iss"
    if ($LASTEXITCODE -eq 0) {
        $setupPath = "$root\Output\EODServiceManager_Setup.exe"
        if (Test-Path $setupPath) {
            $setupItem = Get-Item $setupPath
            $sizeMb = [math]::Round($setupItem.Length / 1MB, 2)
            Write-Host "`n============================================================" -ForegroundColor Green
            Write-Host " SUCCESS! Standalone installer generated successfully:" -ForegroundColor Green
            Write-Host " Path: $setupPath ($sizeMb MB)" -ForegroundColor Yellow
            Write-Host " Ready to copy and install on any Windows PC!" -ForegroundColor Green
            Write-Host "============================================================" -ForegroundColor Green
        }
    } else {
        Write-Warning "Inno Setup compilation failed with code $LASTEXITCODE."
    }
} else {
    Write-Warning "ISCC.exe not found in standard paths. Please compile $root\EODServiceInstaller.iss manually using Inno Setup."
}
