# Task Context
Create the Player List page using TanStack Table. A key requirement is **Gender-Aware Position Rendering**, where the position name (e.g., "Goalkeeper") is translated based on the player's gender (e.g., "Portero" vs. "Portera") using `i18next` context.

# Core References
- **Plan:** [8 - Implementation_Plan_PlayerManagement.md](./8%20-%20Implementation_Plan_PlayerManagement.md)

# Step-by-Step Instructions
1.  **Create `PlayerListPage.tsx`:**
    *   Path: `src/features/players/pages/PlayerListPage.tsx`
    *   Fetch players using `usePlayersApi`.
    *   Define Columns: Name, Age, Position, Division.
2.  **Implement Gender-Aware Cell:**
    *   For the "Position" column:
    *   `Cell: ({ row, value }) => t('positions.' + value.toLowerCase(), { context: row.original.gender.toLowerCase() })`
3.  **Setup i18n Keys:**
    *   Ensure translation files have keys like `positions.goalkeeper_female`.

# Acceptance Criteria
- [ ] `PlayerListPage.tsx` renders a table of players.
- [ ] Position column correctly translates based on gender context.
