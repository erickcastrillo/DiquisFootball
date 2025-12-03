# Task Context
Create the "Smart Import" Wizard on the frontend. This is a multi-step modal that handles file upload and polls for job status/progress.

# Core References
- **Plan:** [17 - Implementation_Plan_DataPortability.md](./17%20-%20Implementation_Plan_DataPortability.md)

# Step-by-Step Instructions
1.  **Create `ImportWizardModal.tsx`:**
    *   Path: `src/features/data-management/components/ImportWizardModal.tsx`.
    *   State: `file`, `jobId`, `jobStatus`.
    *   Step 1: File Upload Input.
    *   Step 2: Progress Bar (Polling `getJobStatus`).
    *   Handle `FailedValidation` (Show Error Report link).
    *   Handle `Completed` (Success message).
2.  **Integrate:**
    *   Add "Import Players" button to `PlayerListPage.tsx` (or similar) to trigger the modal.

# Acceptance Criteria
- [ ] `ImportWizardModal` renders and handles file upload.
- [ ] Polling logic updates progress bar.
- [ ] Validation errors are displayed correctly.
