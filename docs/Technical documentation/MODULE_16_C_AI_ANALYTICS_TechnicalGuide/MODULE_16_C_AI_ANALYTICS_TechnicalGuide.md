# Technical Implementation Guide: Module 16-C (Predictive AI Revenue Guardian)

This document provides a detailed technical guide for implementing the "Predictive AI (The Revenue Guardian)" module. This module focuses on leveraging historical data to provide predictive financial insights, specifically player churn risk and cash flow forecasting.

## 1. Architectural Analysis

This module primarily involves data analysis and aggregation, creating new read-only data artifacts for dashboarding. It will consume data from existing modules and introduce one new entity to store the results of the churn analysis.

### Domain Entities

1.  **`PlayerChurnRisk`** (New entity, in `Diquis.Domain`): Represents the calculated churn risk for a single player, updated via a recurring background job.
    *   `PlayerId` (Guid, required, FK to `ApplicationUser`).
    *   `RiskScore` (enum: `High`, `Medium`).
    *   `TriggeringSignals` (JSON, string): Stores an array of the signals that caused the risk flag (e.g., `["AttendanceSignal", "FinancialSignal"]`).
    *   `LastCalculatedUtc` (DateTime).

2.  **Consumed Entities**: The analysis will read from several existing domain entities:
    *   `ApplicationUser` (for Player and Parent `last_login_date`).
    *   `TrainingAttendance` (from Module 4, to calculate attendance rates).
    *   `FinancialRecord` (from Module 1, for invoice status and cash flow history).

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `PlayerChurnRisk`: A player's risk profile is strictly confined to their academy.

### Permissions & Authorization

| FRS Role | Policy Name | Permissions Claim | Implementation Detail |
| :--- | :--- | :--- | :--- |
| `Academy Owner` | `IsAcademyOwner` | `permission:analytics.predictive.read` | Grants access to both the Churn Prediction and Cash Flow Forecast features. |
| `Financial Admin`| `IsAcademyAdmin` | `permission:analytics.finance.read` | Grants access only to the Cash Flow Forecast feature, not the player-specific churn data. |

## 2. Scaffolding Strategy (CLI)

This module is not a standard CRUD feature set. A dedicated, non-CRUD service will be created manually for the core logic. Scaffolding can be used to generate the base files for the new entity, which will then be adapted.

1.  **Manual Service & Controller Creation**:
    *   Create a folder `Diquis.Application/Services/PredictiveAnalytics`.
    *   Manually create `IPredictiveAnalyticsService.cs`, `PredictiveAnalyticsService.cs`, and a `DTOs` subfolder.
    *   Manually create `PredictiveAnalyticsController.cs` in `Diquis.WebApi/Controllers`.

2.  **(Optional) Scaffolding for the Churn Risk Entity**:
    *To generate the basic DTOs, entity class, and an initial (but to-be-deleted) service for the `PlayerChurnRisk` entity, run the following from `Diquis.Application/Services`.*
    ```bash
    dotnet new nano-service -s PlayerChurnRisk -p PlayerChurnRisks -ap Diquis
    ```
    *The generated `PlayerChurnRiskService` should be deleted, as its logic will be handled by the background job and the new `PredictiveAnalyticsService`.*

## 3. Implementation Plan (Agile Breakdown)

### User Story: Player Churn Prediction Engine
**As an** Academy Owner, **I want to** be alerted to players likely to quit, **so that** I can intervene.

**Technical Tasks:**
1.  **Domain:** Create the `PlayerChurnRisk` entity in `Diquis.Domain/Entities`. Add the `RiskScore` enum (`High`, `Medium`). Ensure it implements `BaseEntity` and `IMustHaveTenant`.
2.  **Persistence:** Add `DbSet<PlayerChurnRisk>` to `ApplicationDbContext`.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddPlayerChurnRiskEntity`
3.  **Application (DTOs):** In `PredictiveAnalytics/DTOs`, create `PlayerAtRiskDto` (`PlayerId`, `PlayerName`, `RiskScore`, `List<string> TriggeringSignals`).
4.  **Infrastructure (Background Job):**
    -   Create a new Hangfire-compatible background job class: `ChurnPredictionJob.cs`.
    -   This job will be scheduled to run nightly (`RecurringJob.AddOrUpdate`).
    -   **Crucial Logic:** The job will iterate through all tenants, and for each tenant, iterate through all active players. For each player, it will:
        -   Check attendance rate over the last 30 days.
        -   Check for overdue invoices > 5 days.
        -   Check parent `last_login_date` > 60 days.
        -   Calculate the `RiskScore` based on the number/strength of signals.
        -   Update or create a `PlayerChurnRisk` record in the database. If a player no longer has risks, their record should be deleted.
5.  **Application (Service):** In `PredictiveAnalyticsService.cs`:
    -   Implement `GetPlayersAtRiskAsync()`: This method will read from the `PlayerChurnRisk` table for the current tenant and return a list of `PlayerAtRiskDto`.
    -   Implement `GenerateReEngagementScriptAsync(Guid playerId)`: Fetches player and parent names, constructs a prompt, and calls an AI service (e.g., OpenAI) to generate the empathetic script.
6.  **API:** In `PredictiveAnalyticsController.cs`:
    -   `GET /api/predictive-analytics/players-at-risk`: Secured with the `IsAcademyOwner` policy. Calls `GetPlayersAtRiskAsync`.
    -   `POST /api/predictive-analytics/re-engagement-script/{playerId}`: Secured with `IsAcademyOwner`. Calls `GenerateReEngagementScriptAsync`.
7.  **UI (React/Client):**
    -   Create a `<PlayersAtRiskWidget />` component for the owner's dashboard.
    -   The widget will fetch data from `/players-at-risk`.
    -   A "Take Action" button on each player row will trigger a modal, which calls the `/re-engagement-script` endpoint and displays the returned text.

### User Story: Cash Flow Forecaster
**As an** Academy Owner, **I need** a three-month cash flow forecast, **so that** I can make better financial decisions.

**Technical Tasks:**
1.  **Application (DTOs):** In `PredictiveAnalytics/DTOs`, create:
    -   `CashFlowDataPointDto` (`DateTime Date`, `decimal Amount`, `bool IsProjection`).
    -   `CashFlowForecastDto` (`List<CashFlowDataPointDto> ForecastData`, `string InsightSummary`).
2.  **Application (Service):** In `PredictiveAnalyticsService.cs`:
    -   Implement `GetCashFlowForecastAsync()`.
    -   **Crucial Logic:**
        1.  Fetch the last 12+ months of `FinancialRecord` data. If insufficient history exists, throw a specific exception (e.g., `InsufficientDataException`).
        2.  Use a time-series analysis library (e.g., a .NET wrapper for Python's `statsmodels` or a dedicated ML library) to generate a 90-day projection.
        3.  Combine historical and projected data into the `List<CashFlowDataPointDto>`.
        4.  Construct a prompt for an AI service summarizing the key findings (e.g., "The data shows a projected dip of X% in December.") to generate the `InsightSummary`.
3.  **API:** In `PredictiveAnalyticsController.cs`:
    -   `GET /api/predictive-analytics/cash-flow-forecast`: Secured with `IsAcademyOwner` and `IsAcademyAdmin` policies. Calls `GetCashFlowForecastAsync`.
4.  **UI (React/Client):**
    -   Create a new page `src/pages/financials/CashFlowForecast.tsx`.
    -   Use a charting library (e.g., Recharts) to render a line graph from the `ForecastData`.
    -   Use a CSS rule or chart library configuration to render the historical part of the line as solid and the projected part as dotted.
    -   Display the `InsightSummary` in a styled text box below the chart.

## 4. Code Specifications (Key Logic)

### `ChurnPredictionJob.cs` - Signal Aggregation Logic

```csharp
// In Diquis.Infrastructure/BackgroundJobs/ChurnPredictionJob.cs (pseudo-code)
public class ChurnPredictionJob
{
    // Inject DbContext, etc.
    
    public async Task Execute(string tenantId)
    {
        // This job would be invoked for each tenant by a master job.
        // Set tenant context
        
        var activePlayers = await _context.Users.Where(u => u.IsActive && u.Role == "Player").ToListAsync();
        
        var riskResults = new List<PlayerChurnRisk>();

        foreach (var player in activePlayers)
        {
            var signals = new List<string>();

            // 1. Attendance Signal
            // bool hasLowAttendance = await _analyticsService.CheckLowAttendance(player.Id, 30, 0.50);
            if (hasLowAttendance) signals.Add("AttendanceSignal");

            // 2. Financial Signal
            // bool hasOverdueInvoice = await _financialService.CheckOverdueInvoices(player.FamilyId, 5);
            if (hasOverdueInvoice) signals.Add("FinancialSignal");
            
            // 3. Engagement Signal
            // bool hasLowEngagement = await _identityService.CheckParentLogin(player.FamilyId, 60);
            if (hasLowEngagement) signals.Add("EngagementSignal");

            if (signals.Any())
            {
                var risk = new PlayerChurnRisk
                {
                    PlayerId = player.Id,
                    RiskScore = (signals.Count >= 2) ? RiskScore.High : RiskScore.Medium,
                    TriggeringSignals = JsonConvert.SerializeObject(signals),
                    LastCalculatedUtc = DateTime.UtcNow
                };
                riskResults.Add(risk);
            }
        }
        
        // Clear old results for the tenant and insert new ones
        // await _context.PlayerChurnRisks.Where(r => r.TenantId == tenantId).ExecuteDeleteAsync();
        // await _context.PlayerChurnRisks.AddRangeAsync(riskResults);
        // await _context.SaveChangesAsync();
    }
}
```

### `PredictiveAnalyticsService.cs` - Re-engagement Script Prompt

```csharp
// Inside PredictiveAnalyticsService.cs

public async Task<string> GenerateReEngagementScriptAsync(Guid playerId)
{
    // Fetch player and parent details
    var player = await _context.Users.FindAsync(playerId);
    var parent = await _context.Users.FindAsync(player.ParentId); // Assuming ParentId link
    var owner = await _context.Users.FindAsync(_currentUserService.UserId);

    string systemPrompt = $@"You are an empathetic and professional assistant for a youth football academy. 
Your task is to generate a short, friendly re-engagement script for an academy owner to send to a parent.

**CONSTRAINTS:**
- The tone must be supportive and conversational, not accusatory.
- The goal is to open a dialogue and gather feedback.
- Dynamically insert the provided names.

**DATA:**
- Parent's Name: {parent.FirstName}
- Player's Name: {player.FirstName}
- Academy Owner's Name: {owner.FirstName}

Generate the script now.";

    // var script = await _aiService.GetCompletionAsync(systemPrompt);
    var script = $"Hi {parent.FirstName}, this is {owner.FirstName} from the academy. I was reviewing our records and wanted to check in to make sure everything is going well for {player.FirstName}. We really value having you as part of our community and would love to hear any feedback you have. Hope to see you at the pitch soon!"; // Placeholder

    return script;
}
```

### `PredictiveAnalyticsService.cs` - Cash Flow Forecast Logic

```csharp
// Inside PredictiveAnalyticsService.cs

public async Task<CashFlowForecastDto> GetCashFlowForecastAsync()
{
    var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);
    var historicalData = await _context.FinancialRecords
        .Where(fr => fr.Date >= twelveMonthsAgo)
        .OrderBy(fr => fr.Date)
        .Select(fr => new { fr.Date, fr.Amount })
        .ToListAsync();

    if (historicalData.Count < 365) // Example threshold
    {
        throw new InsufficientDataException("At least 12 months of financial history are required to generate a forecast.");
    }

    // --- Time Series Forecasting ---
    // This is where you would integrate with a machine learning library or service.
    // 1. Prepare the data (e.g., aggregate by day/week).
    // 2. Call the forecasting model (e.g., SARIMA, Prophet).
    // var projectedPoints = _mlModel.Predict(historicalData, daysToForecast: 90);
    // For now, we'll use placeholder data.
    var projectedPoints = new List<CashFlowDataPointDto>(); // Placeholder
    
    var forecastData = historicalData
        .Select(p => new CashFlowDataPointDto { Date = p.Date, Amount = p.Amount, IsProjection = false })
        .ToList();
    forecastData.AddRange(projectedPoints);

    // --- AI Insight Summary ---
    // string summaryPrompt = "Analyze the following forecast data and provide a one-sentence summary of the main trend for the next 90 days. Data: " + JsonConvert.SerializeObject(forecastData);
    // var insightSummary = await _aiService.GetCompletionAsync(summaryPrompt);
    var insightSummary = "Based on last year's data, our forecast predicts a seasonal cash-flow dip of approximately 40% in December. Plan expenditures accordingly."; // Placeholder

    return new CashFlowForecastDto
    {
        ForecastData = forecastData,
        InsightSummary = insightSummary
    };
}
```
