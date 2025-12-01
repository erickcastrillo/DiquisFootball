# Technical Implementation Guide: Analytics & Reporting

This document provides a detailed technical guide for implementing the features outlined in the "Analytics & Reporting" Functional Requirement Specification (FRS). This module is primarily focused on data aggregation and presentation, not data creation.

## 1. Architectural Analysis

### Domain Entities

This module will **consume existing domain entities** rather than creating new ones. The primary entities involved are:
-   `ApplicationUser` (for players, parents, coaches)
-   `Team`
-   `PlayerTeam` (linking entity)
-   `TrainingSession`
-   `TrainingAttendance` (linking entity)
-   `PlayerSkill` (or similar entity for tracking skill ratings over time)
-   `FinancialRecord`
-   `InventoryItem`

The "Dashboards" and "Reports" mentioned in the FRS are not persistent domain entities but rather **transient View Models (DTOs)** constructed on-the-fly by aggregating data from the core entities.

### Multi-Tenancy Scope

All data queries for this module **must be strictly tenant-scoped**. The boilerplate's existing global query filters on `ApplicationDbContext` will automatically handle this for any entity implementing `IMustHaveTenant`. Special care must be taken for queries that join across multiple entities to ensure the tenant scope is maintained correctly.

### Permissions & Authorization

Access to analytics data is highly role-dependent. This will be enforced at the Application Service level by checking the user's role and at the API level with authorization policies.

| FRS Role | Policy Name | Permissions Claim | Data Scope |
| :--- | :--- | :--- | :--- |
| `academy_owner` | `IsAcademyOwner` | `permission:analytics.business` | Own Academy |
| `academy_admin` | `IsAcademyAdmin` | `permission:analytics.operations`| Own Academy |
| `director_of_football`| `IsDirector` | `permission:analytics.performance`| Own Academy |
| `coach` | `IsCoach` | `permission:analytics.team` | Own Team(s) |
| `parent` / `player` | `IsEndUser` | `permission:analytics.self` | Own/Child's Data |

## 2. Scaffolding Strategy (CLI)

Standard CRUD scaffolding (`dotnet new nano-service`) is **not suitable** for this module, as we are not managing a new domain entity. Instead, a dedicated, non-CRUD service will be created manually.

**Manual Service & Controller Creation:**
1.  Create a folder `Diquis.Application/Services/AnalyticsService`.
2.  Inside, manually create the following files:
    *   `IAnalyticsService.cs`
    *   `AnalyticsService.cs`
    *   A subfolder `DTOs` for all dashboard and report-related data transfer objects.
3.  Manually create a new `AnalyticsController.cs` in `Diquis.WebApi/Controllers`.

## 3. Implementation Plan (Agile Breakdown)

### User Story: Role-Based Dashboards
**As a** user (e.g., `academy_owner`, `coach`), **I want to** see a dashboard with key metrics relevant to my role, **so that** I get a quick overview of performance and status.

**Technical Tasks:**
1.  **Domain:** No changes.
2.  **Persistence:** No changes.
3.  **Application (DTOs):** In `Application/Services/AnalyticsService/DTOs`, define specific DTOs for each role's dashboard.
    -   `AcademyOwnerDashboardDto`: Contains `TotalPlayers` (int), `MonthlyRevenue` (decimal), `TeamCount` (int).
    -   `CoachDashboardDto`: Contains `TeamAttendanceRate` (double), `List<TopPlayerMetricDto>` `TopPerformingPlayers`.
    -   `PlayerDashboardDto`: Contains `PersonalAttendanceRate` (double), `List<SkillProgressionPointDto>` `SkillProgression`.
4.  **Application (Service):** In `AnalyticsService.cs`, implement methods for each dashboard.
    -   `GetAcademyOwnerDashboardAsync()`: Queries `Users`, `FinancialRecords`, and `Teams` tables, scoped by the current `TenantId`, to calculate the required metrics.
    -   `GetCoachDashboardAsync(Guid coachId)`: Queries `TrainingAttendance` and `PlayerSkill` tables, scoped to the teams managed by the specified `coachId`.
    -   `GetPlayerDashboardAsync(Guid playerId)`: Queries the same tables but scoped only to the specified `playerId`.
5.  **API:** In `AnalyticsController.cs`, create a single endpoint that routes to the correct service method based on the user's role.
    -   `GET /api/analytics/dashboard`: This endpoint will inspect the user's claims, determine their primary role (`academy_owner`, `coach`, etc.), and call the corresponding method in `IAnalyticsService`. This avoids creating multiple role-specific API endpoints.
6.  **UI (React/Client):**
    -   Create a unified `src/pages/dashboard/DashboardPage.tsx` component.
    -   This component will fetch data from the `/api/analytics/dashboard` endpoint.
    -   Based on the shape of the returned data, it will conditionally render different widget components (e.g., `<OwnerRevenueWidget />`, `<CoachAttendanceWidget />`).

### User Story: Report Generation
**As an** `academy_admin`, **I need to** generate and export a monthly financial summary, **so that** I can share it with the academy owner.

**Technical Tasks:**
1.  **Domain:** No changes.
2.  **Persistence:** No changes.
3.  **Application (DTOs):** Define DTOs that represent the structure of each report.
    -   `MonthlyFinancialsDto`: `List<FinancialRecord>`, `Totals` (decimal).
    -   `TeamRosterDto`: `TeamName`, `CoachName`, `List<PlayerContactInfo>`.
4.  **Application (Service):** In `AnalyticsService.cs`:
    -   `GenerateFinancialReportAsync(DateTime startDate, DateTime endDate)`: Fetches `FinancialRecord` data within the date range.
    -   `GenerateTeamRosterReportAsync(Guid teamId)`: Fetches player and parent contact info for a specific team.
    -   These methods will return the data DTO, not the file itself.
5.  **API:** In `AnalyticsController.cs`, create endpoints for each report.
    -   `GET /api/analytics/reports/financial`: Secured with `IsAcademyAdmin` policy. Calls the service to get data, then passes it to the `IExcelExportService` (for CSV) or `IPdfExportService`. The controller returns a `FileResult`.
    -   `GET /api/analytics/reports/team-roster`: Secured with `IsDirector` policy. Calls the service to get data, then passes it to the `IPdfExportService`.
6.  **UI (React/Client):**
    -   Create a `src/pages/reports/ReportsPage.tsx`.
    -   This page will display a list of available reports based on the user's role.
    -   Clicking a "Generate" button will trigger a download request to the appropriate API endpoint, which will stream the file back to the browser.

## 4. Code Specifications (Key Logic)

### `AnalyticsService.cs` - Academy Owner Dashboard Aggregation

```csharp
// Inside AnalyticsService.cs

public async Task<AcademyOwnerDashboardDto> GetAcademyOwnerDashboardAsync()
{
    // TenantId is applied automatically via global query filters
    var tenantId = _currentTenantService.TenantId;

    var totalPlayers = await _context.Users
        .Where(u => u.TenantId == tenantId && u.UserRoles.Any(ur => ur.Role.Name == "Player"))
        .CountAsync();

    var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    var monthlyRevenue = await _context.FinancialRecords
        .Where(fr => fr.Date >= startOfMonth)
        .SumAsync(fr => fr.Amount);

    var teamCount = await _context.Teams.CountAsync();

    return new AcademyOwnerDashboardDto
    {
        TotalPlayers = totalPlayers,
        MonthlyRevenue = monthlyRevenue,
        TeamCount = teamCount
    };
}
```

### `AnalyticsController.cs` - Dynamic Dashboard Endpoint

```csharp
// Inside AnalyticsController.cs

[HttpGet("dashboard")]
public async Task<IActionResult> GetDashboard()
{
    if (User.IsInRole("academy_owner"))
    {
        var data = await _analyticsService.GetAcademyOwnerDashboardAsync();
        return Ok(data);
    }
    if (User.IsInRole("coach"))
    {
        var coachId = // get from User claims
        var data = await _analyticsService.GetCoachDashboardAsync(coachId);
        return Ok(data);
    }
    if (User.IsInRole("player"))
    {
        var playerId = // get from User claims
        var data = await _analyticsService.GetPlayerDashboardAsync(playerId);
        return Ok(data);
    }

    return Forbid(); // Or return a default empty dashboard
}
```

### `AnalyticsController.cs` - Report File Generation

```csharp
// Inside AnalyticsController.cs

[HttpGet("reports/financial-summary-csv")]
[Authorize(Policy = "IsAcademyAdmin")]
public async Task<IActionResult> GetFinancialSummaryCsv([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
{
    var reportData = await _analyticsService.GetFinancialReportDataAsync(startDate, endDate);

    // Assuming IExcelExportService is injected
    var fileBytes = await _excelExportService.ExportToCsvAsync(reportData.Records,
        new Dictionary<string, Func<FinancialRecord, object>>
        {
            { "Date", item => item.Date.ToShortDateString() },
            { "Amount", item => item.Amount },
            { "Description", item => item.Description }
        });

    return File(fileBytes, "text/csv", $"FinancialSummary_{DateTime.UtcNow:yyyy-MM-dd}.csv");
}
```