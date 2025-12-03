# Task Context
Define the core domain entities for Academy Operations: `FinancialRecord` and `InventoryItem`. These entities are tenant-specific and must implement `IMustHaveTenant` to ensure strict data isolation.

# Core References
- **Plan:** [7 - Implementation_Plan_AcademyOperations.md](./7%20-%20Implementation_Plan_AcademyOperations.md)
- **Tech Guide:** [AcademyOperations_TechnicalGuide.md](../../Technical%20documentation/AcademyOperations_TechnicalGuide/AcademyOperations_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `FinancialRecord.cs`:**
    *   Path: `Diquis.Domain/Entities/FinancialRecord.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties:
        *   `Amount` (decimal)
        *   `Date` (DateTime)
        *   `Description` (string?)
        *   `ReferenceNumber` (string?)
        *   `PayerId` (Guid)
        *   `TenantId` (string, required)
2.  **Create `InventoryItem.cs`:**
    *   Path: `Diquis.Domain/Entities/InventoryItem.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties:
        *   `Name` (string, required)
        *   `Description` (string?)
        *   `Quantity` (int)
        *   `Category` (string?)
        *   `TenantId` (string, required)
3.  **Prepare for Migration:**
    *   Ensure `ApplicationDbContext` will pick up these entities (usually via assembly scanning or manual `DbSet` addition).

# Acceptance Criteria
- [ ] `FinancialRecord.cs` exists with correct properties.
- [ ] `InventoryItem.cs` exists with correct properties.
- [ ] Both entities implement `IMustHaveTenant`.
- [ ] Solution builds successfully.
