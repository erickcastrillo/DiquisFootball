# Task Context
Create the frontend Analytics Dashboard using `Chart.js`. The dashboard should dynamically render different widgets based on the user's role (Owner vs. Coach).

# Core References
- **Plan:** [15 - Implementation_Plan_AnalyticsReporting.md](./15%20-%20Implementation_Plan_AnalyticsReporting.md)

# Step-by-Step Instructions
1.  **Install Dependencies:**
    *   `npm install chart.js react-chartjs-2`.
2.  **Create `DataWidget.tsx`:**
    *   Path: `src/features/analytics/components/DataWidget.tsx`.
    *   Props: Title, Value, Icon.
3.  **Create `OwnerDashboard.tsx`:**
    *   Path: `src/features/analytics/components/OwnerDashboard.tsx`.
    *   Fetch data from API.
    *   Render widgets (Total Players, Revenue, etc.).
    *   (Optional) Render a Chart.js chart for Revenue trends.
4.  **Create `DashboardPage.tsx`:**
    *   Path: `src/pages/dashboard/DashboardPage.tsx`.
    *   Determine user role.
    *   Render `OwnerDashboard` or `CoachDashboard` (placeholder).

# Acceptance Criteria
- [ ] Dependencies installed.
- [ ] `DataWidget` component created.
- [ ] `OwnerDashboard` fetches and displays data.
- [ ] `DashboardPage` handles role-based rendering.
