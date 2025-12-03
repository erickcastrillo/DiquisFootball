# Task Context
Define the core domain entities for Scouting & Recruitment: `Trialist` and `EvaluationCard`. These entities are tenant-specific and must implement `IMustHaveTenant`.

# Core References
- **Plan:** [13 - Implementation_Plan_ScoutingRecruitment.md](./13%20-%20Implementation_Plan_ScoutingRecruitment.md)
- **Tech Guide:** [ScoutingRecruitment_TechnicalGuide.md](../../Technical%20documentation/ScoutingRecruitment_TechnicalGuide/ScoutingRecruitment_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `Trialist.cs`:**
    *   Path: `Diquis.Domain/Entities/Trialist.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `Name`, `DateOfBirth`, `ContactInfo`, `PreviousClub`, `Status` (Enum), `PromotedToPlayerId` (Nullable Guid), `TenantId`.
2.  **Create `EvaluationCard.cs`:**
    *   Path: `Diquis.Domain/Entities/EvaluationCard.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `TrialistId` (FK), `ScoutId` (FK), `EvaluationDate`, `TechnicalRating` (int), `TacticalRating` (int), `PhysicalRating` (int), `PsychologicalRating` (int), `SummaryNotes`, `PlayerId` (Nullable FK), `TenantId`.
3.  **Define `TrialistStatus` Enum:**
    *   `Active`, `Archived`, `Promoted`.
4.  **Configure Context:**
    *   Ensure `ApplicationDbContext` includes these DbSets.

# Acceptance Criteria
- [ ] `Trialist.cs` and `EvaluationCard.cs` exist.
- [ ] `TrialistStatus` enum is defined.
- [ ] Entities are correctly configured in DbContext.
