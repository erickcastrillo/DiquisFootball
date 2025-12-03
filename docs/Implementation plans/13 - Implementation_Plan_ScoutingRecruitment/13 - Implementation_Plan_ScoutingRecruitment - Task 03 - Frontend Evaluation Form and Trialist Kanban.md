# Task Context
Create the frontend UI for Scouting. This includes a standardized `EvaluationForm` with sliders for ratings and a Kanban-style or List view for managing Trialists.

# Core References
- **Plan:** [13 - Implementation_Plan_ScoutingRecruitment.md](./13%20-%20Implementation_Plan_ScoutingRecruitment.md)

# Step-by-Step Instructions
1.  **Create `EvaluationForm.tsx`:**
    *   Path: `src/features/scouting/components/EvaluationForm.tsx`.
    *   Use `Formik` and `Yup`.
    *   Fields: Technical, Tactical, Physical, Psychological (Sliders 1-10), SummaryNotes (Textarea).
2.  **Create `TrialistList.tsx`:**
    *   Path: `src/features/scouting/components/TrialistList.tsx`.
    *   Use `TanStack Table`.
    *   Columns: Name, Age, Status, Actions (Promote button for Directors).
3.  **Integrate:**
    *   Use in `ScoutingDashboardPage.tsx`.

# Acceptance Criteria
- [ ] `EvaluationForm` renders with validation.
- [ ] `TrialistList` displays trialists.
- [ ] Promote action is available for authorized users.
