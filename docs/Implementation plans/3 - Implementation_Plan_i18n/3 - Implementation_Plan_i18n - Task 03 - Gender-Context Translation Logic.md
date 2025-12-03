# Task Context
Implement and verify the gender-aware translation logic. Spanish requires different terms for roles based on gender (e.g., "Portero" vs "Portera"). We will use `i18next`'s context feature to handle this automatically.

# Core References
- **Plan:** [3 - Implementation_Plan_i18n.md](./3%20-%20Implementation_Plan_i18n.md)

# Step-by-Step Instructions
1.  **Define Translation Keys (Mock/Example):**
    *   Create/Edit `public/locales/es/translation.json` (or equivalent mock for testing).
    *   Add keys with context suffixes:
        *   `"positions.goalkeeper": "Portero"`
        *   `"positions.goalkeeper_female": "Portera"`
2.  **Implement Usage Logic:**
    *   In a component (e.g., `PlayerCard`), determine the context: `const genderContext = player.gender.toLowerCase();`
    *   Call `t('positions.goalkeeper', { context: genderContext })`.
3.  **Create Unit Test (`src/utils/translations.test.ts`):**
    *   Use `vitest`.
    *   Initialize a test instance of `i18next`.
    *   **Test Case 1:** Verify default term (no context) returns male form.
    *   **Test Case 2:** Verify `context: 'female'` returns female form.
    *   **Test Case 3:** Verify fallback behavior (if specific female term missing, return default).

# Acceptance Criteria
- [ ] Translation JSON structure supports `_female` suffix.
- [ ] Code logic correctly passes `context` to the `t` function.
- [ ] Unit tests pass, confirming that "Portera" is returned for female context and "Portero" for default/male.
