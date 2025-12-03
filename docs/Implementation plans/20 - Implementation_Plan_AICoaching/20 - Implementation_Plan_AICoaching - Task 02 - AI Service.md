# Task Context
Implement the `AICoachingService` to orchestrate the generation of training plans and match feedback. This service acts as the bridge between domain data (Team, Inventory, Stats) and the external AI model.

# Core References
- **Plan:** [20 - Implementation_Plan_AICoaching.md](./20%20-%20Implementation_Plan_AICoaching.md)
- **Tech Guide:** [AI_Coaching_Assistant_TechnicalGuide.md](../../Technical%20documentation/AI_Coaching_Assistant_TechnicalGuide/AI_Coaching_Assistant_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `AICoachingService.cs`:**
    *   Path: `Diquis.Application/Services/AI/AICoachingService.cs`.
    *   Inject `ITeamService`, `IInventoryService`, `IAnalyticsService`, `IOpenAIService`, `IFileStorageService`, `ISpeechToTextService`.
2.  **Implement `GenerateSessionPlanAsync`:**
    *   Fetch Team (Age Group) and Inventory (Available Equipment).
    *   Construct Prompt: Include constraints (Age, Equipment, Focus Area).
    *   Call AI.
    *   Append Disclaimer.
    *   Save `TrainingSessionPlan`.
3.  **Implement `GenerateAllFeedbackAsync`:**
    *   Upload/Transcribe Voice Note.
    *   Fetch Player Stats.
    *   Loop Players -> Construct Prompt (Voice Note + Stats) -> Call AI.
    *   Save `PlayerMatchFeedback` (Status: Draft).
4.  **Implement `SendAllFeedbackAsync`:**
    *   Fetch Drafts -> Send via `ICommunicationService` -> Update Status to Sent.

# Acceptance Criteria
- [ ] `AICoachingService` implements orchestration logic.
- [ ] Prompts include context (Inventory, Stats) and constraints.
- [ ] Disclaimer appended to session plans.
