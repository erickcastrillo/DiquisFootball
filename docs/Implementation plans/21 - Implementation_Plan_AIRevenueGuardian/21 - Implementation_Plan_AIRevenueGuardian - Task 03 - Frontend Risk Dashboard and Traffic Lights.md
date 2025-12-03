# Task Context
Create the frontend dashboard widget to visualize churn risk. Use "traffic light" badges (Red/Yellow) to indicate risk levels.

# Core References
- **Plan:** [21 - Implementation_Plan_AIRevenueGuardian.md](./21%20-%20Implementation_Plan_AIRevenueGuardian.md)

# Step-by-Step Instructions
1.  **Create `PlayersAtRiskWidget.tsx`:**
    *   Path: `src/features/ai-revenue/components/PlayersAtRiskWidget.tsx`.
    *   Fetch data from `GetPlayersAtRiskAsync`.
    *   Render list of players.
    *   **Visuals:** Use `Badge` component: Red for High Risk, Yellow for Medium Risk.
    *   Show triggering signals (e.g., "Low Attendance").
    *   "Take Action" button -> Opens Modal with Re-engagement Script.
2.  **Create `CashFlowForecastPage.tsx`:**
    *   Path: `src/pages/financials/CashFlowForecast.tsx`.
    *   Render Line Chart (Historical vs. Projected).
    *   Display AI Insight Summary.

# Acceptance Criteria
- [ ] `PlayersAtRiskWidget` displays correct risk levels and signals.
- [ ] "Take Action" modal generates and shows script.
- [ ] Cash Flow chart renders correctly.
