# Internationalization (i18n) Implementation Guide

## Overview

This project uses Rails i18n with Inertia.js to provide seamless internationalization support for English and Spanish.

## Features

- **2 Languages**: English (en), Spanish (es)
- **Server-side translations**: Managed in Rails locale files
- **Automatic sharing**: Translations automatically available in React via Inertia
- **Type-safe**: TypeScript helper with full type safety
- **Interpolation support**: Dynamic values in translations
- **Locale persistence**: User's language preference saved in session
- **Browser detection**: Auto-detects preferred language from browser

## File Structure

```text
config/
  locales/
    en.yml          # English translations
    es.yml          # Spanish translations

app/
  controllers/
    application_controller.rb  # Locale switching logic
  frontend/
    lib/
      i18n.ts       # TypeScript helper for React components
```

## Usage in React Components

### Basic Usage

```tsx
import { useTranslations } from "@/lib/i18n";

export default function Dashboard() {
  const { t } = useTranslations();

  return (
    <div>
      <h1>{t("app.dashboard.title")}</h1>
      {/* Output: "Dashboard" (en), "Tablero" (es) */}
    </div>
  );
}
```

### With Interpolation

```tsx
import { useTranslations } from "@/lib/i18n";

export default function Dashboard() {
  const { t } = useTranslations();
  const userName = "John Doe";

  return (
    <div>
      <p>{t("app.dashboard.welcome", { name: userName })}</p>
      {/* Output: "Welcome back, John Doe!" (en) */}
      {/* Output: "Bienvenido de nuevo, John Doe!" (es) */}
    </div>
  );
}
```

### Language Switcher Component

```tsx
import { router } from "@inertiajs/react";
import { useTranslations } from "@/lib/i18n";

export function LanguageSwitcher() {
  const { locale, available_locales } = useTranslations();

  const switchLocale = (newLocale: string) => {
    router.visit(window.location.pathname, {
      data: { locale: newLocale },
      preserveState: true,
      preserveScroll: true,
    });
  };

  return (
    <div className="dropdown">
      <button className="btn">
        {locale.toUpperCase()}
      </button>
      <ul className="dropdown-menu">
        {available_locales.map((loc) => (
          <li key={loc.code}>
            <button
              className={locale === loc.code ? "active" : ""}
              onClick={() => switchLocale(loc.code)}
            >
              {loc.name}
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
```

## Adding New Translations

### 1. Add to locale files

```yaml
# config/locales/en.yml
en:
  app:
    your_feature:
      title: "My New Feature"
      description: "This is a description"
```

```yaml
# config/locales/es.yml
es:
  app:
    your_feature:
      title: "Mi Nueva Función"
      description: "Esta es una descripción"
```

### 2. Use in React component

```tsx
const { t } = useTranslations();

<h1>{t("app.your_feature.title")}</h1>
```

## Translation Keys Structure

Current translation namespaces:

- `app.dashboard.*` - Dashboard page translations
- `app.navigation.*` - Sidebar navigation items
- `app.header.*` - Header/navbar translations
- `app.footer.*` - Footer translations
- `common.*` - Common UI elements (save, cancel, etc.)
- `errors.*` - Error messages

## Locale Switching Priority

The system determines the locale in this order:

1. **URL parameter** (`?locale=es`)
2. **Session** (previously selected language)
3. **Browser** (HTTP_ACCEPT_LANGUAGE header)
4. **Default** (English)

## API Reference

### useTranslations() Hook

Returns an object with:

```typescript
{
  t: (key: string, interpolations?: Record<string, any>) => string;
  locale: string;
  available_locales: Array<{ code: string; name: string }>;
  translations: Translations;
}
```

### t() Function

Standalone function for non-hook contexts:

```typescript
import { t } from "@/lib/i18n";

const translatedText = t(translations, "app.dashboard.title");
```

## Best Practices

1. **Use dot notation** for nested keys: `app.dashboard.title`
2. **Keep translations organized** by feature/page
3. **Use interpolation** for dynamic content: `%{name}`
4. **Provide defaults** for new keys during development
5. **Test all languages** before deploying
6. **Use descriptive keys** that indicate context

## Example: Complete Dashboard Translation

```tsx
import { useTranslations } from "@/lib/i18n";

export default function Dashboard() {
  const { t } = useTranslations();

  return (
    <div>
      <h1>{t("app.dashboard.title")}</h1>

      {/* Stats Cards */}
      <div className="stats-grid">
        <div className="card">
          <h3>{t("app.dashboard.stats.total_customers")}</h3>
          <p>1,336</p>
        </div>
        <div className="card">
          <h3>{t("app.dashboard.stats.revenue")}</h3>
          <p>425k</p>
        </div>
      </div>

      {/* Sales Report */}
      <div className="card">
        <h2>{t("app.dashboard.sales_report.title")}</h2>
        <p>{t("app.dashboard.sales_report.subtitle")}</p>
      </div>
    </div>
  );
}
```

## Next Steps

To integrate i18n into your existing components:

1. Import `useTranslations` hook
2. Replace hard-coded strings with `t()` calls
3. Add missing translations to locale files
4. Test language switching
5. Add language switcher to header

## Testing

```typescript
// In tests, you can mock the translations
jest.mock("@/lib/i18n", () => ({
  useTranslations: () => ({
    t: (key: string) => key, // Returns the key for testing
    locale: "en",
    available_locales: [
      { code: "en", name: "English" },
      { code: "es", name: "Español" },
    ],
  }),
}));
```
