# Task Context
Create a reusable React component `AuditHistoryModal` to display the audit trail for a specific record. This modal will fetch data from the API and display the history of changes, including a JSON diff view for old vs. new values.

# Core References
- **Plan:** [4 - Implementation_Plan_SecurityAudit.md](./4%20-%20Implementation_Plan_SecurityAudit.md)

# Step-by-Step Instructions
1.  **Create API Hook:**
    *   Ensure `useAuditApi` exists (or create it) with a method `getAuditHistory(tableName, recordId)`.
2.  **Create Modal Component:**
    *   Path: `src/components/auditing/AuditHistoryModal.tsx`
    *   Props: `show`, `onHide`, `tableName`, `recordId`.
3.  **Implement Logic:**
    *   Use `useEffect` to fetch logs when the modal opens (`show` becomes true).
    *   Show a loading spinner while fetching.
4.  **Render UI:**
    *   Use `react-bootstrap` Modal.
    *   Map through fetched logs.
    *   Display metadata: Action Type, User, Timestamp.
    *   Display changes: Use `react-json-view` (or similar) to show `OldValue` and `NewValue`.
5.  **Integrate (Example):**
    *   (Optional for this task, but good for verification) Add a "View History" button to a management table (e.g., Users) that opens this modal.

# Acceptance Criteria
- [ ] `AuditHistoryModal.tsx` exists.
- [ ] Component fetches data based on props.
- [ ] Component displays a list of audit entries.
- [ ] JSON diff or raw JSON is visible for changes.
