# Task Context
Create the frontend UI for Sports Medicine, emphasizing security. This includes an `InjuryForm` with visual indicators for encrypted fields and ensuring access control is respected.

# Core References
- **Plan:** [14 - Implementation_Plan_SportsMedicine.md](./14%20-%20Implementation_Plan_SportsMedicine.md)

# Step-by-Step Instructions
1.  **Create `InjuryForm.tsx`:**
    *   Path: `src/features/medical/components/InjuryForm.tsx`.
    *   Use `Formik`.
    *   Create `SecureFieldAddon` component (Lock icon).
    *   Apply addon to `BodyPart`, `Diagnosis` fields.
2.  **Implement Access Control:**
    *   Ensure "Add Injury" button is only visible to `director_of_football` (Medical Staff).
    *   Ensure `InjuryList` handles forbidden errors gracefully.
3.  **Integrate:**
    *   Use in `PlayerProfilePage.tsx` (Medical History tab).

# Acceptance Criteria
- [ ] `InjuryForm` renders with lock icons for encrypted fields.
- [ ] UI respects `IsMedicalStaff` permissions.
