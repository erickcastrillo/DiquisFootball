# Diquis Domain Definition: Implementation & Testing Plan

## 1. Executive Summary

This document details the implementation and testing strategy for establishing core domain enumerations within the Diquis platform. A clear and consistent definition of these enums is critical for data integrity and a stable contract between the .NET backend and the React frontend.

This plan establishes the C# enum definitions in the `Diquis.Domain` layer, creates a corresponding set of constants and mapping utilities in the React client, and defines a testing strategy to ensure seamless serialization and deserialization.

## 2. Backend Domain Enum Definition (C#)

As per Clean Architecture principles, all core domain concepts, including enums, will reside in the `Diquis.Domain` project. This ensures they are independent of application, infrastructure, and presentation concerns.

**Action:** Create a new folder `Enums` within the `Diquis.Domain` project and add the following files.

---

**File:** `Diquis.Domain/Enums/SubscriptionTier.cs`
```csharp
namespace Diquis.Domain.Enums;

public enum SubscriptionTier
{
    Grassroots = 0,
    Professional = 1,
    Enterprise = 2
}
```

---

**File:** `Diquis.Domain/Enums/PlayerPosition.cs`
```csharp
namespace Diquis.Domain.Enums;

public enum PlayerPosition
{
    Goalkeeper = 0,
    Defender = 1,
    Midfielder = 2,
    Forward = 3
}
```

---

**File:** `Diquis.Domain/Enums/Gender.cs`
```csharp
namespace Diquis.Domain.Enums;

public enum Gender
{
    Male = 0,
    Female = 1,
    NonBinary = 2
}
```

---

**File:** `Diquis.Domain/Enums/ResourceStatus.cs`
```csharp
namespace Diquis.Domain.Enums;

public enum ResourceStatus
{
    Available = 0,
    OutOfService = 1
}
```

---

**File:** `Diquis.Domain/Enums/InjuryStatus.cs`
```csharp
namespace Diquis.Domain.Enums;

public enum InjuryStatus
{
    Active = 0,
    Closed = 1
}
```

## 3. Frontend Integration (React)

To ensure type safety and provide a foundation for internationalization (i18n), we will create a central file for enum definitions and related utilities in the React client.

**Action:** Create a new file `src/constants/enums.ts` in the React project.

### 3.1. Enum Constants and Mappings

Instead of simple TypeScript `enum`s, we will use constant arrays of objects. This pattern is more flexible and allows us to attach metadata, such as `labelKey` for translation.

**File:** `src/constants/enums.ts`
```typescript
// src/constants/enums.ts

/**
 * Represents the available subscription tiers.
 * Mirrored from Diquis.Domain.Enums.SubscriptionTier
 */
export const SUBSCRIPTION_TIERS = [
    { value: 'Grassroots', labelKey: 'subscriptionTiers.grassroots' },
    { value: 'Professional', labelKey: 'subscriptionTiers.professional' },
    { value: 'Enterprise', labelKey: 'subscriptionTiers.enterprise' },
] as const;

/**
 * Represents the primary player positions.
 * Mirrored from Diquis.Domain.Enums.PlayerPosition
 */
export const PLAYER_POSITIONS = [
    { value: 'Goalkeeper', labelKey: 'playerPositions.goalkeeper' },
    { value: 'Defender', labelKey: 'playerPositions.defender' },
    { value: 'Midfielder', labelKey: 'playerPositions.midfielder' },
    { value: 'Forward', labelKey: 'playerPositions.forward' },
] as const;

/**
 * Represents gender identity.
 * Mirrored from Diquis.Domain.Enums.Gender
 */
export const GENDERS = [
    { value: 'Male', labelKey: 'genders.male' },
    { value: 'Female', labelKey: 'genders.female' },
    { value: 'NonBinary', labelKey: 'genders.nonBinary' },
] as const;

// We can also define simple string enums for simpler cases if needed
export enum ResourceStatus {
    Available = 'Available',
    OutOfService = 'OutOfService',
}

export enum InjuryStatus {
    Active = 'Active',
    Closed = 'Closed',
}

// Type definition for a generic enum option, useful for populating dropdowns
export type EnumOption<T extends readonly { value: string; labelKey: string }[]> = T[number]['value'];

```

### 3.2. Gendered Enum Translation Strategy

The structure above is designed to integrate seamlessly with an i18n library like `react-i18next`. We can create a helper function or a custom hook to resolve the translated label.

**Conceptual Example:** `src/hooks/useTranslatedEnum.ts`
```typescript
import { useTranslation } from 'react-i18next';
import { GENDERS } from '@/constants/enums'; // Adjust path as needed

// This is a conceptual example. The final implementation will depend on the i18n setup.

/**
 * A utility function to get the translated label for a given enum value.
 * In a real component, this would be a custom hook: useTranslatedEnumLabel(value, GENDERS)
 */
export const getGenderLabel = (genderValue: string): string => {
  const { t } = useTranslation();
  const gender = GENDERS.find(g => g.value === genderValue);
  
  if (!gender) {
    return 'Unknown';
  }
  
  // The t() function looks up the 'genders.male' key in the translation files (e.g., en.json)
  return t(gender.labelKey);
};

/*
  Example i18n file (e.g., public/locales/en/translation.json):
  {
    "genders": {
      "male": "Male",
      "female": "Female",
      "nonBinary": "Non-Binary"
    }
  }
*/
```

## 4. Testing Strategy

The primary risk is a mismatch in serialization/deserialization between the API and the client. The following tests will mitigate this risk.

### 4.1. JSON Serialization Configuration (Backend)

First, we must ensure the backend serializes enums as strings, not integers. This is crucial for readability and client-side compatibility.

**Action:** In `Diquis.WebApi/Program.cs`, configure the JSON serializer.

```csharp
// In Diquis.WebApi/Program.cs

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // This converter is essential for the contract between backend and frontend.
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
```

### 4.2. Backend Serialization Unit Test

We will create a unit test to verify that a DTO containing enums is correctly serialized to a JSON string.

**Action:** Create a test in the `Diquis.WebApi.Tests` project.

**File:** `Diquis.WebApi.Tests/Serialization/EnumSerializationTests.cs`
```csharp
using System.Text.Json;
using Diquis.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace Diquis.WebApi.Tests.Serialization;

// A sample DTO for testing purposes
public record PlayerProfileDto(
    string Name,
    PlayerPosition Position,
    Gender Gender
);

public class EnumSerializationTests
{
    [Test]
    public void Enums_Should_Serialize_To_Strings()
    {
        // Arrange
        var playerProfile = new PlayerProfileDto(
            "Alex Doe",
            PlayerPosition.Midfielder,
            Gender.NonBinary
        );

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        // Act
        var json = JsonSerializer.Serialize(playerProfile, options);

        // Assert
        json.Should().Contain(""Position":"Midfielder"");
        json.Should().Contain(""Gender":"NonBinary"");
    }
}
```

### 4.3. Frontend Deserialization Unit Test

We will create a Vitest/Jest test to ensure the frontend can correctly interpret a JSON payload from the API.

**Action:** Create a test file like `src/constants/enums.test.ts`.

```typescript
import { describe, it, expect } from 'vitest';
import { GENDERS, PLAYER_POSITIONS, type EnumOption } from './enums';

// Mock API payload for a player profile
const mockPlayerApiResponse = {
  name: 'Alex Doe',
  position: 'Midfielder', // String value from API
  gender: 'NonBinary',   // String value from API
};

describe('Enum Type Safety', () => {
  it('should correctly map API string to a valid TypeScript enum type', () => {
    // This test simulates assigning an API response to a typed frontend object
    
    type PlayerProfile = {
      name: string;
      position: EnumOption<typeof PLAYER_POSITIONS>;
      gender: EnumOption<typeof GENDERS>;
    };

    const player: PlayerProfile = {
        ...mockPlayerApiResponse,
        // The following lines will pass if the strings from the mock match the 'value' in our constants
        position: mockPlayerApiResponse.position as PlayerProfile['position'],
        gender: mockPlayerApiResponse.gender as PlayerProfile['gender'],
    };

    // Check if the values are correctly assigned
    expect(player.position).toBe('Midfielder');
    expect(player.gender).toBe('NonBinary');
    
    // Check type compatibility (compile-time check, but good to have runtime check too)
    const isPositionValid = PLAYER_POSITIONS.some(p => p.value === player.position);
    expect(isPositionValid).toBe(true);
  });
});
```

### 4.4. End-to-End (E2E) Verification

A final E2E test will confirm the entire flow.

**Scenario:**
1.  **Tool:** Playwright or Cypress.
2.  **Setup:** Create a test user and log in.
3.  **Action:** Navigate to a form for creating a player profile.
4.  **Interact:**
    *   Locate a `<select>` dropdown for "Position".
    *   Verify it contains options like "Goalkeeper", "Defender", etc.
    *   Select "Forward".
5.  **Submit:** Submit the form.
6.  **Assert:**
    *   Intercept the `POST` request to `/api/players` and verify the JSON payload contains `"position":"Forward"`.
    *   Alternatively, navigate to the player's profile page and assert that "Forward" is displayed, confirming the value was saved and retrieved correctly.

This multi-layered testing approach ensures that our domain enums are robust, maintainable, and correctly synchronized across the entire stack.
