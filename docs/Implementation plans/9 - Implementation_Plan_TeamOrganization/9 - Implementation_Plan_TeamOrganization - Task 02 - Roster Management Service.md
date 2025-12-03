# Task Context
Implement the business logic for roster management in `TeamService.cs`. This includes filtering eligible players based on age/gender/assignment status and performing transactional updates to the roster (add/remove players).

# Core References
- **Plan:** [9 - Implementation_Plan_TeamOrganization.md](./9%20-%20Implementation_Plan_TeamOrganization.md)

# Step-by-Step Instructions
1.  **Implement `GetEligiblePlayersAsync`:**
    *   Input: `teamId`.
    *   Logic:
        *   Get Team -> Division.
        *   Calculate min/max birth dates.
        *   Get list of already assigned players (globally for tenant).
        *   Query `Users` (Role=Player, Gender match, Age match, Not assigned).
2.  **Implement `UpdateRosterAsync`:**
    *   Input: `teamId`, `UpdateRosterRequest` (IdsToAdd, IdsToRemove).
    *   Logic:
        *   Start Transaction.
        *   Validate Roster Size (optional but good).
        *   Remove players in `IdsToRemove`.
        *   Add players in `IdsToAdd`.
        *   Commit Transaction.
3.  **Unit Test:**
    *   Create `Diquis.Infrastructure.Tests/Teams/RosterEligibilityTests.cs`.
    *   Verify eligibility filtering logic.

# Acceptance Criteria
- [ ] `TeamService` implements `GetEligiblePlayersAsync` and `UpdateRosterAsync`.
- [ ] Logic correctly filters players and updates DB transactionally.
- [ ] Unit tests pass.
