# publish.ps1 - Builds both projects into publish_forms for Inno Setup packaging
# Run this before compiling EODServiceInstaller.iss

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

Write-Host "Cleaning publish_forms..." -ForegroundColor Cyan
if (Test-Path "$root\publish_forms") {
    Remove-Item "$root\publish_forms" -Recurse -Force
}

Write-Host ""
Write-Host "Publishing EODService (console)..." -ForegroundColor Cyan
dotnet publish "$root\EODService\EODService.csproj" `
    -c Release -r win-x64 --self-contained `
    -o "$root\publish_forms"

if ($LASTEXITCODE -ne 0) { Write-Error "EODService publish failed."; exit 1 }

Write-Host ""
Write-Host "Publishing EODSettingsApp (WinForms)..." -ForegroundColor Cyan
dotnet publish "$root\EODSettingsApp\EODSettingsApp.csproj" `
    -c Release -r win-x64 --self-contained `
    -o "$root\publish_forms"

if ($LASTEXITCODE -ne 0) { Write-Error "EODSettingsApp publish failed."; exit 1 }

Write-Host ""
Write-Host "Done! publish_forms is ready. Now compile EODServiceInstaller.iss with Inno Setup." -ForegroundColor Green
