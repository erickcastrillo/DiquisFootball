# Task Context
Extend the `ApplicationUser` entity in the Domain layer to support player-specific data. This involves adding properties for date of birth, division, parent linkage, and gender. The `Gender` property is critical for localized UI text.

# Core References
- **Plan:** [8 - Implementation_Plan_PlayerManagement.md](./8%20-%20Implementation_Plan_PlayerManagement.md)
- **Tech Guide:** [PlayerManagement_TechnicalGuide.md](../../Technical%20documentation/PlayerManagement_TechnicalGuide/PlayerManagement_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Modify `ApplicationUser.cs`:**
    *   Path: `Diquis.Domain/Entities/Identity/ApplicationUser.cs`
    *   Add `DateOfBirth` (DateOnly?).
    *   Add `DivisionId` (Guid?).
    *   Add `ParentId` (Guid?).
    *   Add `Gender` (Gender Enum).
    *   Add `FirstName` and `LastName` if not already present.
2.  **Define `Gender` Enum:**
    *   Ensure `Diquis.Domain/Enums/Gender.cs` exists (created in Domain Definition plan).
3.  **Prepare for Migration:**
    *   Ensure `BaseDbContext` (Identity context) will pick up these changes.

# Acceptance Criteria
- [ ] `ApplicationUser.cs` includes new properties.
- [ ] `Gender` enum is correctly referenced.
- [ ] Solution builds successfully.
