# Task Context
Initialize the internationalization framework in the React client using `react-i18next`. This includes installing dependencies, configuring the `i18n` instance to load translations from the backend, and creating a UI component to allow users to switch languages.

# Core References
- **Plan:** [3 - Implementation_Plan_i18n.md](./3%20-%20Implementation_Plan_i18n.md)

# Step-by-Step Instructions
1.  **Install Dependencies:**
    *   Run `npm install react-i18next i18next i18next-http-backend i18next-browser-languagedetector`.
2.  **Create Configuration (`src/i18n.ts`):**
    *   Import `i18n`, `initReactI18next`, `HttpApi`, `LanguageDetector`.
    *   Initialize `i18n` with:
        *   `supportedLngs: ['en', 'es']`
        *   `fallbackLng: 'en'`
        *   `contextSeparator: '_'` (Important for gender logic)
        *   `backend: { loadPath: '/api/translations/{{lng}}' }`
3.  **Integrate in `main.tsx`:**
    *   Import `./i18n` to ensure it runs on startup.
4.  **Create `LanguageSwitcher.tsx`:**
    *   Create a component using `react-bootstrap` Dropdown.
    *   Use `useTranslation` hook.
    *   Implement `changeLanguage` function calling `i18n.changeLanguage(lng)`.
    *   List options for English and Español.

# Acceptance Criteria
- [ ] `package.json` includes i18next dependencies.
- [ ] `src/i18n.ts` is created and configured correctly.
- [ ] `src/main.tsx` imports the configuration.
- [ ] `src/components/LanguageSwitcher.tsx` exists and functions.
