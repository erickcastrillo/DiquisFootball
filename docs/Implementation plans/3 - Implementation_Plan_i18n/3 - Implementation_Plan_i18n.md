# Internationalization (i18n) & Localization: Implementation & Testing Plan

## 1. Executive Summary

This document provides a detailed technical implementation plan for integrating internationalization (i18n) and localization into the Diquis platform. The strategy supports the business requirement for a Day 1 launch in English (`en`) and Spanish (`es`), while establishing a scalable framework for future languages.

The plan covers backend configuration for handling cultures, frontend integration using `react-i18next` in the existing Vite project, and a testing strategy focused on ensuring the correctness of translations, including complex gender-aware scenarios.

## 2. Backend Implementation (ASP.NET Core)

The backend will be configured to recognize and handle supported cultures. This is primarily for culture-specific formatting of data processed on the server and for localizing any API-generated messages (e.g., validation errors).

### 2.1. Configure Request Localization Middleware

We will configure the middleware in `Diquis.WebApi/Program.cs` to support English and Spanish as the primary languages. English will serve as the fallback culture.

**Action:** Update `Program.cs`.

```csharp
// In Diquis.WebApi/Program.cs

// 1. Define supported cultures
var supportedCultures = new[] { "en-US", "es-CR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// 2. Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

// 3. Use the middleware
// This should be placed before UseAuthorization and other middleware that might depend on culture.
app.UseRequestLocalization(localizationOptions);

// ... rest of the middleware pipeline
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 2.2. Implement `IStringLocalizer` in Services

To provide localized error messages or other strings from the backend, we will use the `IStringLocalizer<T>` interface. This allows us to decouple text from the code.

**Conceptual Example:** Localizing a validation error in an Application service.

```csharp
// In Diquis.Application/Services/PlayerService/PlayerService.cs

public class PlayerService
{
    private readonly IStringLocalizer<PlayerService> _localizer;

    public PlayerService(IStringLocalizer<PlayerService> localizer)
    {
        _localizer = localizer;
    }

    public void SomePlayerMethod(Player player)
    {
        if (player.Age < 10)
        {
            // The key "PlayerTooYoung" will be looked up in the resource files
            // (e.g., PlayerService.en.resx, PlayerService.es.resx)
            // based on the current request's culture.
            throw new ValidationException(_localizer["PlayerTooYoung", player.Name]);
        }
        // ...
    }
}
```
**Associated Resource File (`PlayerService.es.resx`):**
| Name | Value |
|---|---|
| `PlayerTooYoung` | El jugador {0} es demasiado joven. |

## 3. Frontend Extension (React & `react-i18next`)

The primary responsibility for UI translation lies with the React client. We will use the `react-i18next` ecosystem for a robust solution.

### 3.1. Installation

**Action:** In the React client directory, run the following command:
```bash
npm install react-i18next i18next i18next-http-backend i18next-browser-languagedetector
```

### 3.2. Configuration

**Action:** Create a new configuration file `src/i18n.ts`. This file will initialize `i18next` to fetch translation files from our backend, detect the user's browser language, and set a fallback language.

**File:** `src/i18n.ts`
```typescript
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import HttpApi from 'i18next-http-backend';
import LanguageDetector from 'i18next-browser-languagedetector';

i18n
  .use(HttpApi) // Loads translations from a server (e.g., our API)
  .use(LanguageDetector) // Detects user language
  .use(initReactI18next) // Passes i18n instance to react-i18next
  .init({
    // Supported languages
    supportedLngs: ['en', 'es'],
    
    // The default language to fall back to
    fallbackLng: 'en',
    
    // Enable context for gender-aware translations
    contextSeparator: '_',

    detection: {
      // Order and from where user language should be detected
      order: ['querystring', 'cookie', 'localStorage', 'sessionStorage', 'navigator', 'htmlTag'],
      // Cache user language in localStorage
      caches: ['localStorage'],
    },

    backend: {
      // Path to load translations from, e.g., /api/translations/en
      // Based on the technical guide.
      loadPath: '/api/translations/{{lng}}',
    },
    
    react: {
      useSuspense: true, // Recommended for loading translations
    },
  });

export default i18n;
```
**Action:** Import and use this configuration in the main application entry point (`main.tsx`).
```typescript
// In src/main.tsx
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import './i18n'; // Import the i18n configuration

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
);
```

### 3.3. Gender-Aware Translation with Context

To handle gendered terms in Spanish, we will leverage `i18next`'s **context** feature.

**Action:** Define translation keys in the JSON files with context suffixes.

**File:** `public/locales/es/translation.json` (Example file for backend to serve)
```json
{
  "positions": {
    "goalkeeper": "Portero",
    "goalkeeper_female": "Portera",
    "defender": "Defensa",
    "midfielder": "Mediocampista",
    "forward": "Delantero",
    "forward_female": "Delantera"
  }
}
```

**Action:** Use the `context` option when calling the `t` function in a component.

**File:** `src/components/PlayerCard.tsx` (Conceptual Example)
```typescript
import { useTranslation } from 'react-i18next';
import { Player } from '@/types'; // Assuming a Player type exists with a 'gender' property

interface PlayerCardProps {
  player: Player;
}

export const PlayerCard = ({ player }: PlayerCardProps) => {
  const { t } = useTranslation();

  // Determine the context based on the player's gender
  const genderContext = player.gender.toLowerCase(); // e.g., 'female', 'male'

  return (
    <div>
      <h3>{player.name}</h3>
      <p>
        Position: {t(`positions.${player.position.toLowerCase()}`, { context: genderContext, defaultValue: player.position })}
      </p>
    </div>
  );
};
```

### 3.4. `LanguageSwitcher` Component

This component will allow users to change the language manually.

**Action:** Create the `LanguageSwitcher` component.

**File:** `src/components/LanguageSwitcher.tsx`
```typescript
import { useTranslation } from 'react-i18next';
import { Dropdown } from 'react-bootstrap';

const languages = [
  { code: 'en', name: 'English' },
  { code: 'es', name: 'Español' },
];

export const LanguageSwitcher = () => {
  const { i18n } = useTranslation();

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
  };

  const currentLanguage = languages.find(l => l.code === i18n.language) || languages[0];

  return (
    <Dropdown>
      <Dropdown.Toggle variant="secondary" id="language-switcher">
        {currentLanguage.name}
      </Dropdown.Toggle>

      <Dropdown.Menu>
        {languages.map(lang => (
          <Dropdown.Item 
            key={lang.code}
            onClick={() => changeLanguage(lang.code)}
            active={i18n.language === lang.code}
          >
            {lang.name}
          </Dropdown.Item>
        ))}
      </Dropdown.Menu>
    </Dropdown>
  );
};
```

## 4. Testing Strategy

The most critical aspect to test is the correct resolution of contextual translations.

### 4.1. Unit Test: Gender Context Translation

**Action:** Create a unit test file to verify that passing a gender context returns the correct Spanish translation.

**File:** `src/utils/translations.test.ts`
```typescript
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import { describe, it, expect, beforeAll } from 'vitest';

// Mock translation resources
const resources = {
  es: {
    translation: {
      positions: {
        goalkeeper: 'Portero',
        goalkeeper_female: 'Portera',
        forward: 'Delantero',
        forward_female: 'Delantera',
      },
    },
  },
  en: {
    translation: {
      positions: {
        goalkeeper: 'Goalkeeper',
        forward: 'Forward',
      },
    },
  },
};

// Initialize a test instance of i18next before tests run
beforeAll(() => {
  i18n.use(initReactI18next).init({
    lng: 'es',
    fallbackLng: 'en',
    resources,
    contextSeparator: '_',
  });
});

describe('Gender-Aware Translations', () => {
  it('should return the default Spanish term ("Portero") for goalkeeper with no context', () => {
    const translated = i18n.t('positions.goalkeeper');
    expect(translated).toBe('Portero');
  });

  it('should return the female-specific Spanish term ("Portera") for goalkeeper when context is "female"', () => {
    const translated = i18n.t('positions.goalkeeper', { context: 'female' });
    expect(translated).toBe('Portera');
  });

  it('should return the male-specific (default) Spanish term ("Delantero") for forward when context is "male"', () => {
    const translated = i18n.t('positions.forward', { context: 'male' });
    expect(translated).toBe('Delantero');
  });
  
  it('should fall back to the default term if a specific context does not exist', () => {
    // Assuming 'midfielder_female' does not exist, it should return 'midfielder'
    i18n.addResourceBundle('es', 'translation', { positions: { midfielder: 'Mediocampista' } }, true, true);
    const translated = i18n.t('positions.midfielder', { context: 'female' });
    expect(translated).toBe('Mediocampista');
  });
});
```
This test suite provides high confidence that the i18n context logic is configured correctly and behaves as expected, directly fulfilling the core testing requirement.
