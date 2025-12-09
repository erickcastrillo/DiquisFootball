# PowerShell script to rename Diquis.Infrastructure.Jobs to Diquis.BackgroundJobs
# Run this from the solution root directory

$ErrorActionPreference = "Stop"

$oldName = "Diquis.Infrastructure.Jobs"
$newName = "Diquis.BackgroundJobs"
$oldNamespace = "Diquis.Infrastructure.Jobs"
$newNamespace = "Diquis.BackgroundJobs"
$solutionRoot = "C:\Users\erick\source\repos\Diquis"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Project Rename: $oldName -> $newName" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check if old directory exists
$oldDir = Join-Path $solutionRoot $oldName
if (-not (Test-Path $oldDir)) {
    Write-Host "ERROR: Directory $oldDir does not exist!" -ForegroundColor Red
    exit 1
}

Write-Host "[1/8] Verified old directory exists" -ForegroundColor Green

# Step 2: Rename project directory
$newDir = Join-Path $solutionRoot $newName
Write-Host "[2/8] Renaming directory: $oldName -> $newName" -ForegroundColor Yellow
Rename-Item -Path $oldDir -NewName $newName -Force

# Step 3: Rename .csproj file
$oldCsproj = Join-Path $newDir "$oldName.csproj"
$newCsproj = Join-Path $newDir "$newName.csproj"
Write-Host "[3/8] Renaming .csproj file" -ForegroundColor Yellow
Rename-Item -Path $oldCsproj -NewName "$newName.csproj" -Force

# Step 4: Update namespace in all .cs files in the renamed project
Write-Host "[4/8] Updating namespaces in .cs files" -ForegroundColor Yellow
$csFiles = Get-ChildItem -Path $newDir -Filter "*.cs" -Recurse
foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match $oldNamespace) {
        $content = $content -replace [regex]::Escape($oldNamespace), $newNamespace
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Updated: $($file.Name)" -ForegroundColor Gray
    }
}

# Step 5: Update launchSettings.json
Write-Host "[5/8] Updating launchSettings.json" -ForegroundColor Yellow
$launchSettings = Join-Path $newDir "Properties\launchSettings.json"
if (Test-Path $launchSettings) {
    $content = Get-Content $launchSettings -Raw
    $content = $content -replace [regex]::Escape($oldName), $newName
    Set-Content -Path $launchSettings -Value $content -NoNewline
    Write-Host "  Updated launchSettings.json" -ForegroundColor Gray
}

# Step 6: Update appsettings files
Write-Host "[6/8] Updating appsettings files" -ForegroundColor Yellow
$appsettingsFiles = Get-ChildItem -Path $newDir -Filter "appsettings*.json"
foreach ($file in $appsettingsFiles) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match $oldName) {
        $content = $content -replace [regex]::Escape($oldName), $newName
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Updated: $($file.Name)" -ForegroundColor Gray
    }
}

# Step 7: Add project to solution if not already there
Write-Host "[7/8] Checking solution file" -ForegroundColor Yellow
$slnFile = Join-Path $solutionRoot "Diquis.sln"
$slnContent = Get-Content $slnFile -Raw

if ($slnContent -notmatch [regex]::Escape($newName)) {
    Write-Host "  Adding project to solution..." -ForegroundColor Yellow
    dotnet sln $slnFile add $newCsproj
} else {
    Write-Host "  Project already in solution" -ForegroundColor Gray
}

# Step 8: Update documentation references
Write-Host "[8/8] Searching for documentation references..." -ForegroundColor Yellow
$docFiles = Get-ChildItem -Path (Join-Path $solutionRoot "docs") -Filter "*.md" -Recurse -ErrorAction SilentlyContinue
foreach ($file in $docFiles) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match $oldName) {
        $content = $content -replace [regex]::Escape($oldName), $newName
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Updated: $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "? Rename completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Open Visual Studio and reload the solution" -ForegroundColor White
Write-Host "2. Clean and rebuild the solution" -ForegroundColor White
Write-Host "3. Update any project references if needed" -ForegroundColor White
Write-Host "4. Test the application" -ForegroundColor White
Write-Host ""
