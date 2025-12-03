# Task Context
Enhance the Super Admin Dashboard in the React client with data visualizations. Implement a chart component to display tenant distribution by tier using `chart.js` and `react-chartjs-2`.

# Core References
- **Plan:** [6 - Implementation_Plan_SaaS_BackOffice.md](./6%20-%20Implementation_Plan_SaaS_BackOffice.md)

# Step-by-Step Instructions
1.  **Install Dependencies:**
    *   `npm install chart.js react-chartjs-2`.
2.  **Create Chart Component:**
    *   Path: `src/components/admin/GlobalRevenueChart.tsx`
    *   Logic:
        *   Fetch data using `useSassAnalyticsApi` (create hook if needed).
        *   Transform `TierDistribution` dictionary into Chart.js data format.
        *   Render `Bar` chart.
3.  **Integrate into Dashboard:**
    *   Add `GlobalRevenueChart` to `src/pages/admin/AdminDashboard.tsx`.
    *   Ensure it's only rendered for `SystemSuperAdmin`.

# Acceptance Criteria
- [ ] Dependencies installed.
- [ ] `GlobalRevenueChart.tsx` exists and renders data.
- [ ] Dashboard displays the chart.
