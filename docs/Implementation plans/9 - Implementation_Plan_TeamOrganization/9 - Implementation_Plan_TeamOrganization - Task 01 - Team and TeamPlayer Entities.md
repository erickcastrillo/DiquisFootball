# Task Context
Define the core domain entities for Team Organization: `Team` and `TeamPlayer`. Also, extend the existing `Division` entity with a `Gender` property. These entities are tenant-specific and must implement `IMustHaveTenant`.

# Core References
- **Plan:** [9 - Implementation_Plan_TeamOrganization.md](./9%20-%20Implementation_Plan_TeamOrganization.md)
- **Tech Guide:** [TeamOrganization_TechnicalGuide.md](../../Technical%20documentation/TeamOrganization_TechnicalGuide/TeamOrganization_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `Team.cs`:**
    *   Path: `Diquis.Domain/Entities/Team.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `Name`, `CoachId` (FK), `DivisionId` (FK), `TeamPlayers` (Collection), `TenantId`.
2.  **Create `TeamPlayer.cs`:**
    *   Path: `Diquis.Domain/Entities/TeamPlayer.cs`
    *   Implement `IMustHaveTenant`.
    *   Properties: `TeamId` (FK), `PlayerId` (FK), `TenantId`.
3.  **Update `Division.cs`:**
    *   Path: `Diquis.Domain/Entities/Division.cs`
    *   Add `Gender` property (Enum).
4.  **Configure Context:**
    *   In `ApplicationDbContext`, configure composite key for `TeamPlayer` (`TeamId`, `PlayerId`).

# Acceptance Criteria
- [ ] `Team.cs` and `TeamPlayer.cs` exist.
- [ ] `Division.cs` has `Gender` property.
- [ ] `TeamPlayer` has composite key configured in DbContext.
