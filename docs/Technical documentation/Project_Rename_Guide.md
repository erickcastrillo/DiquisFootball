# Renaming Diquis.BackgroundJobs to Diquis.BackgroundJobs

## Overview
This document outlines the process to rename the `Diquis.BackgroundJobs` project to `Diquis.BackgroundJobs` to better reflect its purpose and maintain implementation-agnostic naming.

## Prerequisites
- Close Visual Studio completely
- Commit any uncommitted changes to Git
- Create a backup or new branch for this change

## Option 1: Automated Rename (Using PowerShell Script)

### Steps:
1. **Close Visual Studio completely**

2. **Run the PowerShell script:**
   ```powershell
   cd C:\Users\erick\source\repos\Diquis
   .\rename-project.ps1
   ```

3. **Open Visual Studio and reload the solution**

4. **Clean and rebuild:**
   ```
   Build > Clean Solution
   Build > Rebuild Solution
   ```

5. **Verify everything works:**
   - Both projects should start without errors
   - Check Hangfire dashboard at https://localhost:7298/hangfire
   - Test job enqueueing at https://localhost:7250/api/test/enqueue-job

## Option 2: Manual Rename

### Step 1: Rename Directory and Files
1. Navigate to `C:\Users\erick\source\repos\Diquis\`
2. Rename folder: `Diquis.BackgroundJobs` ? `Diquis.BackgroundJobs`
3. Inside the renamed folder, rename: `Diquis.BackgroundJobs.csproj` ? `Diquis.BackgroundJobs.csproj`

### Step 2: Update Namespaces
Update all `.cs` files in the renamed project to use the new namespace:

**Find:** `namespace Diquis.BackgroundJobs`  
**Replace with:** `namespace Diquis.BackgroundJobs`

Files to update:
- `Program.cs`
- `Data\JobsDbInitializer.cs`
- `Data\DiquisInfrastructureJobsContext.cs`
- `Extensions\ServiceCollectionExtensions.cs`
- `Extensions\WebApplicationExtensions.cs`
- `Middleware\HangfireAuthenticationMiddleware.cs`
- `Middleware\HangfireCustomAuthorizationFilter.cs`
- All other `.cs` files in the project

### Step 3: Update Project Configuration Files

**launchSettings.json:**
Change profile name from `Diquis.BackgroundJobs` to `Diquis.BackgroundJobs`:
```json
{
  "profiles": {
    "Diquis.BackgroundJobs": {
      // ... rest of configuration
    }
  }
}
```

### Step 4: Add Project to Solution
```powershell
cd C:\Users\erick\source\repos\Diquis
dotnet sln Diquis.sln add Diquis.BackgroundJobs\Diquis.BackgroundJobs.csproj
```

### Step 5: Update Documentation
Search and replace in documentation files:
- `docs\Technical documentation\Testing_Hangfire_Jobs.md`
- `docs\Technical documentation\Hangfire_OpenTelemetry_Integration.md`
- `docs\Technical documentation\Jobs_Authentication_Flow.md`
- Any implementation plan documents

**Find:** `Diquis.BackgroundJobs`  
**Replace with:** `Diquis.BackgroundJobs`

### Step 6: Update Using Statements (if needed)
Check if any files have explicit using statements:
```csharp
using Diquis.BackgroundJobs.Extensions;
```
Replace with:
```csharp
using Diquis.BackgroundJobs.Extensions;
```

## Post-Rename Verification Checklist

- [ ] Solution builds without errors
- [ ] Both Diquis.WebApi and Diquis.BackgroundJobs start successfully
- [ ] Hangfire dashboard is accessible at https://localhost:7298/hangfire
- [ ] Test job can be enqueued successfully
- [ ] Test job executes and completes
- [ ] OpenTelemetry traces are captured correctly
- [ ] No broken references in the solution
- [ ] All tests pass
- [ ] Git status shows expected changes

## Expected File Changes

### Files to be Renamed:
- Directory: `Diquis.BackgroundJobs/` ? `Diquis.BackgroundJobs/`
- Project: `Diquis.BackgroundJobs.csproj` ? `Diquis.BackgroundJobs.csproj`

### Files to be Modified:
- `Diquis.sln` (add new project reference)
- `Diquis.BackgroundJobs/Properties/launchSettings.json`
- All `.cs` files in `Diquis.BackgroundJobs/` (namespace changes)
- Documentation files (3+ markdown files)

### Files Not Changed:
- `Diquis.Infrastructure/BackgroundJobs/*` - These keep their namespace
- `Diquis.WebApi/Program.cs` - Already references `Diquis.Infrastructure.BackgroundJobs`
- Connection strings and app settings (no namespace references)

## Troubleshooting

### "Project not found" error:
- Ensure the project was added to the solution file
- Run: `dotnet sln list` to verify

### Build errors about missing types:
- Clean the solution: `dotnet clean`
- Delete `bin/` and `obj/` folders
- Rebuild: `dotnet build`

### Hangfire dashboard not accessible:
- Check the launchSettings.json profile name matches
- Verify applicationUrl hasn't changed
- Ensure both projects are set to run in multi-project startup

## Rollback Procedure

If issues arise:
1. `git checkout .` to revert all changes
2. Or manually rename back:
   - `Diquis.BackgroundJobs` ? `Diquis.BackgroundJobs`
   - `Diquis.BackgroundJobs.csproj` ? `Diquis.BackgroundJobs.csproj`
   - Revert namespace changes
   - Remove from solution and re-add old project

## Benefits of This Rename

? **Implementation-agnostic** - Not tied to Hangfire  
? **Clear purpose** - Immediately communicates its role  
? **Consistent naming** - Follows Diquis.[Purpose] pattern  
? **Future-proof** - Easy to swap implementations if needed  
? **Better DDD alignment** - Separates job definitions (Infrastructure) from execution (BackgroundJobs)
