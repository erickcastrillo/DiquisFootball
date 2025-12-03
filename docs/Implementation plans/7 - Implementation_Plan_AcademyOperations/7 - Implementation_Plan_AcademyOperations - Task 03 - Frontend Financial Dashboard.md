# Task Context
Create the frontend "Financials" feature area. This includes a dashboard with summary cards (Total Revenue, Monthly Revenue) and a form to log new payments with client-side validation using Formik and Yup.

# Core References
- **Plan:** [7 - Implementation_Plan_AcademyOperations.md](./7%20-%20Implementation_Plan_AcademyOperations.md)

# Step-by-Step Instructions
1.  **Create Folder Structure:**
    *   `src/features/financials/components/`, `hooks/`, `pages/`.
2.  **Create `SummaryCard.tsx`:**
    *   Simple Bootstrap Card to display a title, value, and icon.
3.  **Create `FinancialDashboard.tsx`:**
    *   Fetch summary data using `useFinancialsApi` (create hook).
    *   Render `SummaryCard` components.
4.  **Create `LogPaymentForm.tsx`:**
    *   Use `Formik` and `Yup`.
    *   Schema: `amount` > 0, `payerId` required, `date` required.
    *   Fields: Payer (select), Amount (number), Date, Description.
    *   Submit handler calls API.

# Acceptance Criteria
- [ ] Feature folder structure created.
- [ ] Dashboard renders summary data.
- [ ] Payment form validates input and submits to API.
