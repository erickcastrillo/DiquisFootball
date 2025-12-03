# Task Context
Implement the backend business logic for inventory tracking and auditing in `AssetService.cs`. This includes calculating available quantities and performing transactional updates that log every change.

# Core References
- **Plan:** [12 - Implementation_Plan_AssetManagement.md](./12%20-%20Implementation_Plan_AssetManagement.md)

# Step-by-Step Instructions
1.  **Implement `GetAssetsAsync` (DTO Projection):**
    *   Return `AssetDto` with `AvailableQuantity`.
    *   `AvailableQuantity = TotalQuantity - AssignedQuantity`.
    *   `AssignedQuantity` calculated via subquery count of `PlayerAssets`.
2.  **Implement `AssignAssetToPlayerAsync`:**
    *   Input: `assetId`, `playerId`, `identifier`.
    *   Logic:
        *   Start Transaction.
        *   Check stock availability (`Assigned < Total`).
        *   Create `PlayerAsset`.
        *   Log `AssetAuditEntry` (ChangeType: Assignment, QuantityChange: -1).
        *   Commit Transaction.
3.  **Unit Test:**
    *   Create `Diquis.Infrastructure.Tests/Assets/AssetAssignmentTests.cs`.
    *   Verify stock deduction and audit entry creation.

# Acceptance Criteria
- [ ] `AssetService` implements inventory tracking and assignment logic.
- [ ] `AvailableQuantity` is correctly calculated.
- [ ] Assignments are atomic and audited.
- [ ] Unit tests pass.
