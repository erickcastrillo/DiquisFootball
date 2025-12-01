# Technical Implementation Guide: Module 16-D (AI Governance & Safety Protocols)

This document provides a detailed technical guide for implementing the AI Governance and Safety Framework. Unlike feature-specific modules, this guide outlines architectural patterns, cross-cutting concerns, and mandatory development practices to be enforced across all AI-enabled features in the Diquis platform.

## 1. Architectural Analysis

This module introduces a new logging entity to support the Human-in-the-Loop (HITL) mandate and codifies several critical architectural principles.

### Domain Entities

1.  **`AIInteractionLog`** (New entity, in `Diquis.Domain`): Creates an immutable audit trail for high-consequence AI actions, capturing the "propose and dispose" workflow.
    *   `AIModule` (string, required, e.g., "AutomatedMatchReport", "GdpAnonymization").
    *   `ProposedActionType` (enum: `ExternalCommunication`, `FinancialTransaction`, `DestructiveOperation`).
    *   `ProposedData` (JSON, string): A snapshot of the AI-generated content or proposed action (e.g., the list of emails to be sent, the record IDs to be anonymized).
    *   `Status` (enum: `Proposed`, `Approved`, `Rejected`).
    *   `ReviewedByUserId` (Guid, nullable, FK to `ApplicationUser`): The user who made the decision.
    *   `ReviewedAtUtc` (DateTime, nullable): The timestamp of the decision.

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `AIInteractionLog`: All AI-driven actions are performed within the context of a single academy and must be logged as such.

### Permissions & Authorization

This module leverages existing roles as the "humans in the loop." The key is to ensure that only authorized users can approve specific high-consequence actions.

| FRS Action | Approving Role(s) | Required Policy/Claim |
| :--- | :--- | :--- |
| Send Automated Match Report | `Coach`, `Director of Football` | `permission:coaching.ai.approve` |
| Execute GDPR Anonymization | `Academy Owner` | `permission:player.anonymize` |
| Approve Financial Transaction | `Academy Owner`, `Academy Admin` | `permission:finance.ai.approve` |

## 2. Scaffolding Strategy (CLI)

While most of this module is about patterns, scaffolding can create the base for managing the new log entity. This controller would primarily be for `super_user` auditing purposes.

Execute from `Diquis.Application/Services`:
```bash
dotnet new nano-service -s AIInteractionLog -p AIInteractionLogs -ap Diquis
dotnet new nano-controller -s AIInteractionLog -p AIInteractionLogs -ap Diquis
```
**Note:** The generated `AIInteractionLogService` will be used internally by other services to log events, not for direct user-facing CRUD operations. The controller should be secured for `super_user` access only.

## 3. Implementation Plan (Governance Rule Breakdown)

This implementation plan is broken down by the core governance rules from the FRS.

### Rule 1: Tenant Data Isolation (The "Chinese Wall")
**As an** Architect, **I must** enforce strict data separation for all AI queries, **so that** we prevent data leakage between tenants and comply with our security promise.

**Technical Tasks:**
1.  **Data Access Layer Mandate:**
    -   All services that gather data for AI features (e.g., `PredictiveAnalyticsService`, `AICoachingService`) **must** use dependency injection to receive an `ApplicationDbContext` instance.
    -   Developers are forbidden from using the `BaseDbContext` for AI-related business data queries, as it does not have the automatic tenant filtering for most entities.
2.  **Code Review Enforcement:**
    -   A mandatory item on all Pull Request templates for AI-related features must be: `"[ ] I have verified that all data queries are performed against ApplicationDbContext and that tenant query filters are not disabled."`
3.  **Automated Testing:**
    -   An integration test suite, `AITenantIsolationTests.cs`, must be created.
    -   This suite will contain a test for each AI feature that fetches data.
    -   Each test must:
        1.  Seed data for two distinct tenants, Tenant A and Tenant B.
        2.  Authenticate as a user from Tenant A.
        3.  Execute the AI data-gathering method.
        4.  Assert that the returned data contains records **only** from Tenant A.
        5.  Assert that no records from Tenant B are present in the result.

### Rule 2: The "Human-in-the-Loop" (HITL) Mandate
**As a** Developer, **I must** implement a two-step "propose-then-confirm" workflow for all high-consequence AI actions, **so that** the system is auditable and user-controlled.

**Technical Tasks:**
1.  **Domain & Persistence:**
    -   Create the `AIInteractionLog` entity and its corresponding `DbSet` in `ApplicationDbContext`.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddAIInteractionLogEntity`
2.  **Generic HITL Service Pattern:**
    -   **Generate/Propose Step:** Any method that generates a high-consequence action (e.g., `AICoachingService.GenerateAllFeedbackAsync`) must be modified.
        -   It will **not** execute the action directly (e.g., send emails).
        -   It will save the generated content to its primary entity with a `Status` of `Draft` (e.g., `PlayerMatchFeedback.Status = Draft`).
        -   It will create and save a new `AIInteractionLog` record with `ProposedData` containing the generated content, and `Status = Proposed`. It returns the `Id` of this log entry.
    -   **Review Step:** A new API endpoint (e.g., `GET /api/ai-coaching/match-feedback/review/{interactionLogId}`) will fetch the `AIInteractionLog` and its `ProposedData` for the UI.
    -   **Confirm/Dispose Step:** A new API endpoint (e.g., `POST /api/ai-coaching/match-feedback/approve`) will accept the `interactionLogId`. The corresponding service method will:
        1.  Find the `AIInteractionLog` record.
        2.  Verify the user has the correct permissions to approve.
        3.  Update the `AIInteractionLog` status to `Approved`, setting `ReviewedByUserId` and `ReviewedAtUtc`.
        4.  **Only then**, enqueue the background job to perform the actual action (e.g., `_jobService.Enqueue<IEmailService>(...)`).
        5.  Update the original draft entity's status to `Sent` or `Executing`.
3.  **UI (React) - Mandatory Review Component:**
    -   Create a generic `<ReviewAndConfirmList />` component.
    -   This component displays the list of proposed actions.
    -   It features a scrollable container.
    -   **Crucial:** A final "Approve & Execute" button must be `disabled` by default and only become enabled when the user has scrolled to the very bottom of the list, ensuring they have had the opportunity to see all items.

### Rule 3: Liability Disclaimers
**As a** Frontend Developer, **I must** display a standard, non-dismissible disclaimer on all UI components showing AI-generated advisory content, **so that** the user understands their responsibility.

**Technical Tasks:**
1.  **UI (React):**
    -   Create a new reusable component: `src/components/ui/AIDisclaimer.tsx`.
    -   This component will accept a `type` prop (`'coaching' | 'financial' | 'medical'`) and render the specific disclaimer text from the FRS.
    -   The component will be styled with a muted text color (`#4A5568`) and slightly smaller font size to be distinct but legible.
2.  **Implementation Mandate:** This component must be added to the following existing and future feature pages:
    -   `TrainingSessionGenerator.tsx` (Module 16-B): Place `<AIDisclaimer type="coaching" />` directly below the generated plan.
    -   `CashFlowForecast.tsx` (Module 16-C): Place `<AIDisclaimer type="financial" />` directly below the forecast chart.
    -   `PlayersAtRiskWidget.tsx` (Module 16-C): Place `<AIDisclaimer type="financial" />` in the widget's footer.

## 4. Code Specifications (Key Logic)

### Tenant Isolation Integration Test (`AITenantIsolationTests.cs`)

```csharp
[Fact]
public async Task PredictiveAnalytics_GetPlayersAtRisk_ShouldNotLeakDataFromOtherTenants()
{
    // 1. Arrange
    // Seed Tenant A with a player 'PlayerA' at high risk.
    // Seed Tenant B with a player 'PlayerB' at high risk.
    var clientForTenantA = await CreateAuthenticatedClientForTenantAsync("tenant-a-owner@test.com");

    // 2. Act
    // Make a call to the predictive analytics endpoint.
    var response = await clientForTenantA.GetAsync("/api/predictive-analytics/players-at-risk");
    response.EnsureSuccessStatusCode();
    var playersAtRisk = await response.Content.ReadFromJsonAsync<List<PlayerAtRiskDto>>();

    // 3. Assert
    // Using FluentAssertions
    playersAtRisk.Should().NotBeNull();
    playersAtRisk.Should().HaveCount(1);
    playersAtRisk.First().PlayerName.Should().Be("PlayerA");
    playersAtRisk.Should().NotContain(p => p.PlayerName == "PlayerB");
}
```

### HITL Service Pattern (`AICoachingService.cs`)

```csharp
// Inside AICoachingService.cs or similar

// Step 1: Propose
public async Task<Guid> GenerateFeedbackAsync(GenerateFeedbackRequest request)
{
    // ... logic to generate a list of personalized messages ...
    var generatedMessages = new List<FeedbackMessage>(); // Populate this list
    
    // Create the log entry
    var logEntry = new AIInteractionLog
    {
        AIModule = "AutomatedMatchReport",
        ProposedActionType = ProposedActionType.ExternalCommunication,
        ProposedData = JsonConvert.SerializeObject(generatedMessages),
        Status = AIInteractionStatus.Proposed
        // TenantId is set automatically
    };

    await _context.AIInteractionLogs.AddAsync(logEntry);
    await _context.SaveChangesAsync();
    
    // Return the ID for the review step
    return logEntry.Id;
}

// Step 2: Dispose (Approve)
public async Task ApproveAndSendFeedbackAsync(Guid interactionLogId)
{
    var logEntry = await _context.AIInteractionLogs.FindAsync(interactionLogId);
    if (logEntry == null || logEntry.Status != AIInteractionStatus.Proposed)
    {
        throw new InvalidOperationException("This action is not available for approval.");
    }

    // Check permissions
    // if (!_currentUser.HasPermission("permission:coaching.ai.approve")) throw new ForbiddenException();

    // Update the log
    logEntry.Status = AIInteractionStatus.Approved;
    logEntry.ReviewedByUserId = _currentUserService.UserId;
    logEntry.ReviewedAtUtc = DateTime.UtcNow;

    // Enqueue the actual work
    var messages = JsonConvert.DeserializeObject<List<FeedbackMessage>>(logEntry.ProposedData);
    foreach (var message in messages)
    {
        // _jobService.Enqueue<ICommunicationService>(s => s.SendEmailAsync(message.Recipient, message.Content));
    }
    
    await _context.SaveChangesAsync();
}
```

### Disclaimer Component (`AIDisclaimer.tsx`)

```typescript
// In src/components/ui/AIDisclaimer.tsx
import React from 'react';

const disclaimerText = {
  coaching: "This content is AI-generated. The Certified Professional (Coach) remains fully responsible for verifying its accuracy and ensuring the physical safety of all participants before and during use.",
  financial: "This content is an AI-generated prediction based on historical data and is not a guarantee of future outcomes. The Certified Professional (Owner/Accountant) remains fully responsible for all financial and operational decisions.",
  medical: "This content is AI-generated and for informational purposes only. The Certified Professional (Doctor/Physio) remains fully responsible for any diagnosis or treatment decisions."
};

interface AIDisclaimerProps {
  type: keyof typeof disclaimerText;
  className?: string;
}

export const AIDisclaimer: React.FC<AIDisclaimerProps> = ({ type, className }) => {
  return (
    <div className={`text-xs text-slate-400 p-2 border-l-2 border-slate-600 ${className}`}>
      <p>{disclaimerText[type]}</p>
    </div>
  );
};
```
