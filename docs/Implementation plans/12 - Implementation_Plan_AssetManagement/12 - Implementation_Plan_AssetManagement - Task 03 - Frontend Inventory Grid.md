# Task Context
Create the frontend Inventory Grid using `TanStack Table`. The grid should display asset details and use conditional styling to highlight low-stock items (e.g., warning color if available < 10, danger if 0).

# Core References
- **Plan:** [12 - Implementation_Plan_AssetManagement.md](./12%20-%20Implementation_Plan_AssetManagement.md)

# Step-by-Step Instructions
1.  **Create `InventoryGrid.tsx`:**
    *   Path: `src/features/assets/components/InventoryGrid.tsx`.
    *   Fetch assets using `useAssetsApi`.
    *   Define Columns: Name, Category, Total, Assigned, Available.
2.  **Implement Color-Coding:**
    *   In the "Available" column cell renderer:
    *   If `available <= 10`, apply `text-warning`.
    *   If `available === 0`, apply `text-danger`.
3.  **Integrate:**
    *   Use in `AssetsPage.tsx`.

# Acceptance Criteria
- [ ] `InventoryGrid` renders asset data.
- [ ] Low stock items are visually highlighted.
- [ ] Zero stock items are visually highlighted.
