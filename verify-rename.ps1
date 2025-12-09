# Verification script to check if project rename was successful
# Run this after renaming to verify all changes

$ErrorActionPreference = "Continue"

$newName = "Diquis.BackgroundJobs"
$oldName = "Diquis.Infrastructure.Jobs"
$solutionRoot = "C:\Users\erick\source\repos\Diquis"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Project Rename Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$passedChecks = 0
$failedChecks = 0

# Check 1: New directory exists
Write-Host "[Check 1] Verifying new directory exists..." -ForegroundColor Yellow
$newDir = Join-Path $solutionRoot $newName
if (Test-Path $newDir) {
    Write-Host "  ? Directory '$newName' exists" -ForegroundColor Green
    $passedChecks++
} else {
    Write-Host "  ? Directory '$newName' not found" -ForegroundColor Red
    $failedChecks++
}

# Check 2: Old directory should not exist
Write-Host "[Check 2] Verifying old directory removed..." -ForegroundColor Yellow
$oldDir = Join-Path $solutionRoot $oldName
if (-not (Test-Path $oldDir)) {
    Write-Host "  ? Old directory '$oldName' removed" -ForegroundColor Green
    $passedChecks++
} else {
    Write-Host "  ? Old directory '$oldName' still exists!" -ForegroundColor Red
    $failedChecks++
}

# Check 3: New .csproj file exists
Write-Host "[Check 3] Verifying .csproj file..." -ForegroundColor Yellow
$newCsproj = Join-Path $newDir "$newName.csproj"
if (Test-Path $newCsproj) {
    Write-Host "  ? Project file '$newName.csproj' exists" -ForegroundColor Green
    $passedChecks++
} else {
    Write-Host "  ? Project file '$newName.csproj' not found" -ForegroundColor Red
    $failedChecks++
}

# Check 4: No old namespace references in .cs files
Write-Host "[Check 4] Checking for old namespace references..." -ForegroundColor Yellow
$oldNamespaceFound = $false
if (Test-Path $newDir) {
    $csFiles = Get-ChildItem -Path $newDir -Filter "*.cs" -Recurse
    foreach ($file in $csFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -match "namespace $oldName") {
            Write-Host "  ? Old namespace found in: $($file.Name)" -ForegroundColor Red
            $oldNamespaceFound = $true
        }
    }
}
if (-not $oldNamespaceFound) {
    Write-Host "  ? No old namespace references found" -ForegroundColor Green
    $passedChecks++
} else {
    $failedChecks++
}

# Check 5: launchSettings.json updated
Write-Host "[Check 5] Verifying launchSettings.json..." -ForegroundColor Yellow
$launchSettings = Join-Path $newDir "Properties\launchSettings.json"
if (Test-Path $launchSettings) {
    $content = Get-Content $launchSettings -Raw
    if ($content -match $newName -and $content -notmatch [regex]::Escape($oldName)) {
        Write-Host "  ? launchSettings.json updated correctly" -ForegroundColor Green
        $passedChecks++
    } else {
        Write-Host "  ? launchSettings.json still contains old name" -ForegroundColor Red
        $failedChecks++
    }
} else {
    Write-Host "  ? launchSettings.json not found" -ForegroundColor Yellow
    $failedChecks++
}

# Check 6: Project in solution
Write-Host "[Check 6] Checking if project is in solution..." -ForegroundColor Yellow
$slnFile = Join-Path $solutionRoot "Diquis.sln"
if (Test-Path $slnFile) {
    $slnContent = Get-Content $slnFile -Raw
    if ($slnContent -match [regex]::Escape($newName)) {
        Write-Host "  ? Project is in solution file" -ForegroundColor Green
        $passedChecks++
    } else {
        Write-Host "  ? Project not found in solution file" -ForegroundColor Yellow
        Write-Host "    Run: dotnet sln add $newCsproj" -ForegroundColor Gray
        $failedChecks++
    }
} else {
    Write-Host "  ? Solution file not found" -ForegroundColor Red
    $failedChecks++
}

# Check 7: Build test
Write-Host "[Check 7] Testing project build..." -ForegroundColor Yellow
if (Test-Path $newCsproj) {
    $buildOutput = dotnet build $newCsproj --no-restore 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Project builds successfully" -ForegroundColor Green
        $passedChecks++
    } else {
        Write-Host "  ? Project build failed" -ForegroundColor Red
        Write-Host "    Run 'dotnet build' for details" -ForegroundColor Gray
        $failedChecks++
    }
} else {
    Write-Host "  ? Cannot test build - project file not found" -ForegroundColor Red
    $failedChecks++
}

# Check 8: Documentation updated
Write-Host "[Check 8] Checking documentation..." -ForegroundColor Yellow
$docsPath = Join-Path $solutionRoot "docs"
$oldNameInDocs = $false
if (Test-Path $docsPath) {
    $mdFiles = Get-ChildItem -Path $docsPath -Filter "*.md" -Recurse
    foreach ($file in $mdFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -match [regex]::Escape($oldName)) {
            Write-Host "  ? Old name found in: $($file.Name)" -ForegroundColor Yellow
            $oldNameInDocs = $true
        }
    }
}
if (-not $oldNameInDocs) {
    Write-Host "  ? Documentation updated (or no references found)" -ForegroundColor Green
    $passedChecks++
} else {
    Write-Host "  ? Some documentation files may need updating" -ForegroundColor Yellow
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verification Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Passed: $passedChecks" -ForegroundColor Green
Write-Host "Failed: $failedChecks" -ForegroundColor $(if ($failedChecks -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($failedChecks -eq 0) {
    Write-Host "? All checks passed! Rename completed successfully." -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Open Visual Studio and reload the solution" -ForegroundColor White
    Write-Host "2. Set both projects as startup projects" -ForegroundColor White
    Write-Host "3. Run and test the application" -ForegroundColor White
} else {
    Write-Host "? Some checks failed. Please review and fix the issues above." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Common fixes:" -ForegroundColor Cyan
    Write-Host "- Run the rename-project.ps1 script" -ForegroundColor White
    Write-Host "- Manually update remaining files" -ForegroundColor White
    Write-Host "- Add project to solution: dotnet sln add $newCsproj" -ForegroundColor White
}
Write-Host ""
