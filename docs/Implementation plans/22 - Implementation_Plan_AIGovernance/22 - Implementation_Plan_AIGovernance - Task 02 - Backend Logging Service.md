# Task Context
Implement the "Propose & Dispose" pattern for high-consequence AI actions. This involves modifying services to log a proposal instead of executing immediately, and providing an approval mechanism.

# Core References
- **Plan:** [22 - Implementation_Plan_AIGovernance.md](./22%20-%20Implementation_Plan_AIGovernance.md)
- **Tech Guide:** [MODULE_16_D_AI_GOVERNANCE_TechnicalGuide.md](../../Technical%20documentation/MODULE_16_D_AI_GOVERNANCE_TechnicalGuide/MODULE_16_D_AI_GOVERNANCE_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `AIInteractionLogService.cs` (Internal):**
    *   Path: `Diquis.Application/Services/AI/AIInteractionLogService.cs`.
    *   Methods: `LogProposalAsync`, `ApproveProposalAsync`.
2.  **Modify `AICoachingService` (Example):**
    *   Update `GenerateFeedbackAsync` to call `LogProposalAsync` and return the Log ID.
    *   Implement `ApproveAndSendFeedbackAsync` which calls `ApproveProposalAsync` and then executes the action (sends emails).
3.  **Unit Test:**
    *   Create `AICoachingServiceTests.cs`.
    *   Verify that `GenerateFeedbackAsync` creates a log and DOES NOT send emails.

# Acceptance Criteria
- [ ] `AIInteractionLogService` created.
- [ ] "Propose & Dispose" pattern implemented in `AICoachingService` (or similar).
- [ ] Unit tests verify the HITL workflow.
