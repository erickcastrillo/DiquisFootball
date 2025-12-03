# Task Context
Define the core domain entities for Asset Management: `Asset`, `PlayerAsset`, and `AssetAuditEntry`. These entities are tenant-specific and must implement `IMustHaveTenant`. `AssetAuditEntry` is crucial for tracking all quantity changes.

# Core References
- **Plan:** [12 - Implementation_Plan_AssetManagement.md](./12%20-%20Implementation_Plan_AssetManagement.md)
- **FRS:** [AssetManagement_FRS.md](../../Business%20Requirements/AssetManagement_FRS/AssetManagement_FRS.md)

# Step-by-Step Instructions
1.  **Create `Asset.cs`:**
    *   Path: `Diquis.Domain/Entities/Asset.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `Name`, `Category`, `TotalQuantity`, `TenantId`.
2.  **Create `PlayerAsset.cs`:**
    *   Path: `Diquis.Domain/Entities/PlayerAsset.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `AssetId` (FK), `PlayerId` (FK), `AssignedIdentifier` (e.g., Jersey #), `DateAssigned`, `TenantId`.
3.  **Create `AssetAuditEntry.cs`:**
    *   Path: `Diquis.Domain/Entities/AssetAuditEntry.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `AssetId` (FK), `UserId` (FK), `ChangeType` (Enum), `QuantityChange`, `NewTotalQuantity`, `Reason`, `TenantId`.
4.  **Define `AuditChangeType` Enum:**
    *   `InitialStock`, `Assignment`, `Return`, `Correction`, `Retired`.
5.  **Configure Context:**
    *   Ensure `ApplicationDbContext` includes these DbSets.

# Acceptance Criteria
- [ ] `Asset.cs`, `PlayerAsset.cs`, `AssetAuditEntry.cs` exist.
- [ ] `AuditChangeType` enum is defined.
- [ ] Entities are correctly configured in DbContext.
