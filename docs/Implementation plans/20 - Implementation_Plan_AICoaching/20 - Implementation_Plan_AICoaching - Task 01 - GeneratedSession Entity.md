# Task Context
Define the domain entities required to store AI-generated content. This includes `TrainingSessionPlan` (for session plans) and `PlayerMatchFeedback` (for personalized parent messages).

# Core References
- **Plan:** [20 - Implementation_Plan_AICoaching.md](./20%20-%20Implementation_Plan_AICoaching.md)
- **Tech Guide:** [AI_Coaching_Assistant_TechnicalGuide.md](../../Technical%20documentation/AI_Coaching_Assistant_TechnicalGuide/AI_Coaching_Assistant_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `TrainingSessionPlan.cs`:**
    *   Path: `Diquis.Domain/Entities/TrainingSessionPlan.cs`.
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `TeamId`, `CreatedByCoachId`, `FocusArea`, `GeneratedContent` (Markdown), `AiPromptContext` (JSON snapshot), `TenantId`.
2.  **Create `PlayerMatchFeedback.cs`:**
    *   Path: `Diquis.Domain/Entities/PlayerMatchFeedback.cs`.
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `MatchId`, `PlayerId`, `ParentId`, `GeneratedMessage`, `Status` (Enum: Draft, Reviewed, Sent, Discarded), `TenantId`.
3.  **Configure Context:**
    *   Add `DbSet`s to `ApplicationDbContext`.
    *   Create migration `AddAiCoachingEntities`.

# Acceptance Criteria
- [ ] `TrainingSessionPlan` and `PlayerMatchFeedback` entities created.
- [ ] `ApplicationDbContext` updated.
- [ ] Migration created.
