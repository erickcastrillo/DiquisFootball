# Task Context
Define the `AIInteractionLog` entity to create an immutable audit trail for high-consequence AI actions. This entity supports the "Human-in-the-Loop" (HITL) workflow.

# Core References
- **Plan:** [22 - Implementation_Plan_AIGovernance.md](./22%20-%20Implementation_Plan_AIGovernance.md)
- **Tech Guide:** [MODULE_16_D_AI_GOVERNANCE_TechnicalGuide.md](../../Technical%20documentation/MODULE_16_D_AI_GOVERNANCE_TechnicalGuide/MODULE_16_D_AI_GOVERNANCE_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `AIInteractionLog.cs`:**
    *   Path: `Diquis.Domain/Entities/AIInteractionLog.cs`.
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `AIModule`, `ProposedActionType` (Enum), `ProposedData` (JSON), `Status` (Enum: Proposed, Approved, Rejected), `ReviewedByUserId`, `ReviewedAtUtc`, `TenantId`.
2.  **Configure Context:**
    *   Add `DbSet<AIInteractionLog>` to `ApplicationDbContext`.
    *   Create migration `AddAIInteractionLogEntity`.

# Acceptance Criteria
- [ ] `AIInteractionLog` entity created.
- [ ] `ApplicationDbContext` updated.
- [ ] Migration created.
