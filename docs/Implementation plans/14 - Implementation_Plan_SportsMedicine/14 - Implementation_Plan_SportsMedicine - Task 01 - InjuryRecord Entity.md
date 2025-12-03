# Task Context
Define the core domain entity for Sports Medicine: `InjuryRecord`. This entity is tenant-specific and must implement `IMustHaveTenant`. It also requires the `[Auditable]` attribute for compliance.

# Core References
- **Plan:** [14 - Implementation_Plan_SportsMedicine.md](./14%20-%20Implementation_Plan_SportsMedicine.md)
- **Tech Guide:** [SportsMedicine_TechnicalGuide.md](../../Technical%20documentation/SportsMedicine_TechnicalGuide/SportsMedicine_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `InjuryRecord.cs`:**
    *   Path: `Diquis.Domain/Entities/InjuryRecord.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Attributes: `[Auditable]`.
    *   Properties: `PlayerId` (FK), `DateOfInjury`, `InjuryStatus` (Enum), `RtpStatus` (Enum), `BodyPart` (Encrypted), `InjuryType` (Encrypted), `Diagnosis` (Encrypted), `MechanismOfInjury` (Encrypted), `RehabilitationLogs` (Collection), `TenantId`.
2.  **Define Enums:**
    *   `InjuryStatus`: Active, Closed.
    *   `RtpStatus`: NotCleared, ClearedForNonContact, ClearedForFullTraining.
3.  **Configure Context:**
    *   Ensure `ApplicationDbContext` includes `DbSet<InjuryRecord>`.

# Acceptance Criteria
- [ ] `InjuryRecord.cs` exists with `[Auditable]` attribute.
- [ ] Enums are defined.
- [ ] Entity is correctly configured in DbContext.
