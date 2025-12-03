# Task Context
Create the Roster Builder UI using a drag-and-drop interface. This allows coaches to easily move players between the "Eligible" list and the "Current Roster" list.

# Core References
- **Plan:** [9 - Implementation_Plan_TeamOrganization.md](./9%20-%20Implementation_Plan_TeamOrganization.md)

# Step-by-Step Instructions
1.  **Install Dependencies:**
    *   `npm install @dnd-kit/core @dnd-kit/sortable`.
2.  **Create `RosterBuilder.tsx`:**
    *   Path: `src/features/teams/components/RosterBuilder.tsx`.
    *   State: `eligiblePlayers`, `rosterPlayers`.
    *   Use `DndContext` and `SortableContext`.
    *   Implement `handleDragEnd` to move items between lists.
    *   Implement `handleSaveChanges` to call API.
3.  **Create `PlayerColumn.tsx`:**
    *   Helper component to render a sortable list of players.
4.  **Integrate:**
    *   Use in `RosterManagementPage.tsx`.

# Acceptance Criteria
- [ ] Dependencies installed.
- [ ] `RosterBuilder` component works with drag-and-drop.
- [ ] UI correctly updates state and calls API on save.
