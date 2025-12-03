# Task Context
Implement the business logic to automatically assign a player to an age division based on their date of birth. This logic resides in the `PlayerService` and is triggered during registration and profile updates.

# Core References
- **Plan:** [8 - Implementation_Plan_PlayerManagement.md](./8%20-%20Implementation_Plan_PlayerManagement.md)

# Step-by-Step Instructions
1.  **Create/Update `PlayerService.cs`:**
    *   Path: `Diquis.Application/Services/Players/PlayerService.cs`
    *   Implement `RegisterPlayerAsync`:
        *   Check for duplicates (Name + DOB).
        *   Create `ApplicationUser`.
        *   Call `UpdatePlayerDivisionAsync`.
    *   Implement `UpdatePlayerDivisionAsync` (private):
        *   Calculate age: `Today.Year - DOB.Year` (adjust for birthday).
        *   Query `_context.Divisions` for matching range (`MinAge <= age <= MaxAge`).
        *   Set `player.DivisionId`.
2.  **Unit Test:**
    *   Create `Diquis.Application.Tests/Services/PlayerServiceTests.cs`.
    *   Test cases:
        *   Player fits into a division.
        *   Player is too old/young (uncategorized).
        *   Birthday boundary conditions.

# Acceptance Criteria
- [ ] `PlayerService` implements registration and division logic.
- [ ] Unit tests verify age calculation and division assignment.
