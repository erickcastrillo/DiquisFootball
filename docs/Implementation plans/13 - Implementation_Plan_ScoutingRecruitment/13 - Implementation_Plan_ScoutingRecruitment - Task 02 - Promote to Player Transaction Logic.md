# Task Context
Implement the atomic transaction logic to promote a `Trialist` to a full `ApplicationUser` player. This involves creating the player record, updating the trialist status, and back-filling historical evaluation cards.

# Core References
- **Plan:** [13 - Implementation_Plan_ScoutingRecruitment.md](./13%20-%20Implementation_Plan_ScoutingRecruitment.md)

# Step-by-Step Instructions
1.  **Create `ScoutingOrchestrationService.cs`:**
    *   Path: `Diquis.Application/Services/Scouting/ScoutingOrchestrationService.cs`.
    *   Inject `ApplicationDbContext` and `IPlayerService`.
2.  **Implement `PromoteTrialistToPlayerAsync`:**
    *   Input: `trialistId`, `PromoteTrialistRequest`.
    *   Logic:
        *   Start Transaction.
        *   Validate Trialist (Active?).
        *   Call `_playerService.RegisterPlayerAsync`.
        *   Update `Trialist` (`Status = Promoted`, `PromotedToPlayerId = newId`).
        *   Update `EvaluationCards` (`PlayerId = newId`).
        *   Commit Transaction.
3.  **Unit Test:**
    *   Create `Diquis.Infrastructure.Tests/Scouting/PromotionIntegrityTests.cs`.
    *   Verify atomicity and data linking.

# Acceptance Criteria
- [ ] `ScoutingOrchestrationService` implements promotion logic.
- [ ] Transaction correctly updates Trialist, creates Player, and links Evaluations.
- [ ] Unit tests pass.
