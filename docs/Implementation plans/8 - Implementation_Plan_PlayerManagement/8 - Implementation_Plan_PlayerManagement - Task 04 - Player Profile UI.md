# Task Context
Create a comprehensive Player Profile page with a tabbed interface. This page displays personal info, football skills, and biometrics. It must handle conditional rendering based on the user's role (e.g., parents can edit personal info, coaches can edit skills).

# Core References
- **Plan:** [8 - Implementation_Plan_PlayerManagement.md](./8%20-%20Implementation_Plan_PlayerManagement.md)

# Step-by-Step Instructions
1.  **Create `PlayerProfilePage.tsx`:**
    *   Path: `src/features/players/pages/PlayerProfilePage.tsx`
    *   Use `react-bootstrap` Tabs.
    *   Tabs: "Personal Info", "Football Skills", "Biometrics".
2.  **Create Tab Components:**
    *   `PersonalInfoTab.tsx`: Form for name, DOB, parent info.
    *   `FootballSkillsTab.tsx`: Matrix/List of skills and ratings.
    *   `BiometricsTab.tsx`: Height, Weight charts/data.
3.  **Implement Role-Based Access:**
    *   Use `useAuth` to check roles.
    *   Disable/Hide edit buttons if user lacks permission (e.g., Parent cannot edit Skills).

# Acceptance Criteria
- [ ] `PlayerProfilePage` renders with tabs.
- [ ] Tab components display correct data.
- [ ] UI respects role-based permissions (mocked or real).
