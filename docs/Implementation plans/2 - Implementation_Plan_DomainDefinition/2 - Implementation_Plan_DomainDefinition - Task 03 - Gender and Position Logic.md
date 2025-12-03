# Task Context
Implement the logic to handle translation and type-safe usage of the Gender and Player Position enums in the frontend. This involves creating a utility or hook to resolve the translated labels based on the current language, ensuring the UI displays user-friendly text instead of raw enum values.

# Core References
- **Plan:** [2 - Implementation_Plan_DomainDefinition.md](./2%20-%20Implementation_Plan_DomainDefinition.md)

# Step-by-Step Instructions
1.  **Create Hook/Utility:** Create `src/hooks/useTranslatedEnum.ts` (or similar utility file).
2.  **Implement `getGenderLabel`:**
    *   Import `useTranslation` from `react-i18next`.
    *   Import `GENDERS` from `src/constants/enums.ts`.
    *   Create a function (or hook) that takes a `genderValue` (string).
    *   Find the corresponding object in `GENDERS`.
    *   Return `t(gender.labelKey)` or a fallback if not found.
3.  **Implement `getPositionLabel`:**
    *   Similar logic for `PLAYER_POSITIONS`.
4.  **Unit Test (Frontend):**
    *   Create `src/constants/enums.test.ts`.
    *   Test that API string values (e.g., 'Midfielder') correctly map to the frontend types.
    *   Verify that the constants contain the expected values.

# Acceptance Criteria
- [ ] `src/hooks/useTranslatedEnum.ts` (or equivalent) exists.
- [ ] Logic exists to translate Gender and Position values using `react-i18next`.
- [ ] `src/constants/enums.test.ts` exists and passes, verifying type safety and mapping.
