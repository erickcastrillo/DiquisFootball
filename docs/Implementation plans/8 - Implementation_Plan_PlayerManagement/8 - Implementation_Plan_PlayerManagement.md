# Player Management: Implementation & Testing Plan

## 1. Executive Summary

This document outlines the comprehensive implementation and testing strategy for the Player Management module. This module is central to the Diquis platform, providing the tools for academy staff to manage player registration, profiles, and development tracking.

The plan details the necessary extensions to the core `ApplicationUser` entity, the implementation of critical backend business logic for age calculation, the creation of a modern and responsive React-based user interface for player lists and profiles, and a testing strategy focused on validating the automatic assignment of players to age divisions.

## 2. Architectural Blueprint

The "Player" in our system is not a new top-level entity but is an `ApplicationUser` assigned a "Player" role. We will extend this existing entity to include player-specific attributes.

### 2.1. `ApplicationUser` Entity Extension

**Action:** Extend the existing `ApplicationUser` entity in `Diquis.Domain` with the following properties, as specified in the technical guide and augmented by the UI requirements.

**File:** `Diquis.Domain/Entities/Identity/ApplicationUser.cs` (enhancement)
```csharp
using Diquis.Domain.Enums; // Assuming Gender enum exists

namespace Diquis.Domain.Entities.Identity;

[Auditable] // Mark for audit logging
public class ApplicationUser : IdentityUser
{
    // ... existing properties
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    // --- Player-Specific Extensions ---

    /// <summary>
    /// The player's date of birth. Used for age calculation.
    /// </summary>
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// The ID of the Division (age category) the player belongs to.
    /// This is calculated automatically.
    /// </summary>
    public Guid? DivisionId { get; set; }
    
    /// <summary>
    /// Foreign key to another ApplicationUser who is the parent/guardian.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// CRUCIAL: The player's gender, used to drive localized UI text
    /// for gendered languages like Spanish (e.g., Portero vs. Portera).
    /// </summary>
    public Gender Gender { get; set; }

    // Other player fields like ProfilePictureUrl, PrimaryPositionId etc. would also be added here.
}
```
**Note:** The addition of the `Gender` enum is a crucial augmentation to the technical guide's specification, driven by the requirement for localized, gender-aware UI strings.

### 2.2. Supporting Entities

The following entities from the technical guide are also key to this module:
-   **`Division`**: Defines academy-specific age categories (e.g., "Under-13s") with `MinAge` and `MaxAge` properties.
-   **`Position`**: A lookup table for football positions (e.g., "Midfielder").
-   **`Skill`**: A lookup table for assessable skills (e.g., "Dribbling").
-   **`PlayerSkill`**: A join table linking a player to a skill with a specific `Rating`.

## 3. Backend Implementation: Automatic Age Calculation

A core piece of business logic is the automatic assignment of a player to a `Division` based on their age. This logic will be implemented as a private method within the `PlayerService` and called during registration and profile updates.

**Action:** Implement the age calculation logic in `PlayerService.cs`. This implementation is based on the specification in the technical guide.

**File:** `Diquis.Application/Services/Players/PlayerService.cs`
```csharp
public class PlayerService : IPlayerService
{
    private readonly ApplicationDbContext _context;
    // ... other services

    public async Task RegisterPlayerAsync(RegisterPlayerRequest request)
    {
        // ... logic to check for duplicates and create parent/player users ...

        var player = new ApplicationUser { /* ... properties from request ... */ };

        // After creating the user object, calculate their division
        await UpdatePlayerDivisionAsync(player);

        await _userManager.CreateAsync(player);
        // ...
    }

    private async Task UpdatePlayerDivisionAsync(ApplicationUser player)
    {
        if (player.DateOfBirth is null)
        {
            player.DivisionId = null;
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int age = today.Year - player.DateOfBirth.Value.Year;
        
        // Decrement age if the birthday hasn't occurred yet this year
        if (player.DateOfBirth.Value > today.AddYears(-age))
        {
            age--;
        }

        // Find the division in the current tenant's context
        var matchingDivision = await _context.Divisions
            .FirstOrDefaultAsync(d => age >= d.MinAge && age <= d.MaxAge);

        player.DivisionId = matchingDivision?.Id;
    }
}
```

## 4. Frontend Implementation (React)

A new feature folder will encapsulate all UI for player management.

**Action:** Create the new feature folder `src/features/players`.

### 4.1. Player List with Gender-Aware Localization

The player list will use `TanStack Table` to display data. The "Position" column will demonstrate gender-aware translation by leveraging the `i18next` context feature.

**File:** `src/features/players/pages/PlayerListPage.tsx`
```tsx
import { useMemo } from 'react';
import { useTable, useSortBy } from 'react-table'; // Or TanStack Table equivalents
import { useTranslation } from 'react-i18next';
import { usePlayersApi } from '@/features/players/hooks/usePlayersApi';

export const PlayerListPage = () => {
  const { t } = useTranslation();
  const { data: players = [] } = usePlayersApi().useGetPlayers();

  const columns = useMemo(() => [
    { Header: 'Name', accessor: 'fullName' },
    { Header: 'Age', accessor: 'age' },
    {
      Header: 'Position',
      accessor: 'primaryPosition.name',
      // CRUCIAL: This cell renderer uses the 'gender' property to provide context.
      Cell: ({ row, value }) => {
        const genderContext = row.original.gender?.toLowerCase(); // e.g., 'female'
        const translationKey = `positions.${value?.toLowerCase()}`; // e.g., 'positions.goalkeeper'
        return t(translationKey, { context: genderContext, defaultValue: value });
      },
    },
    // ... other columns
  ], [t]);

  // ... TanStack Table instance setup ...

  return (
    <div>
      <h2>Player Roster</h2>
      {/* ... Table rendering JSX ... */}
    </div>
  );
};
```

### 4.2. Player Profile with Tabbed Interface

The player profile page will use a tabbed layout to organize the large amount of information.

**File:** `src/features/players/pages/PlayerProfilePage.tsx`
```tsx
import { Tabs, Tab } from 'react-bootstrap';
import { useParams } from 'react-router-dom';
import { usePlayersApi } from '@/features/players/hooks/usePlayersApi';

// Import tab content components
import { PersonalInfoTab } from '../components/PersonalInfoTab';
import { FootballSkillsTab } from '../components/FootballSkillsTab';
import { BiometricsTab } from '../components/BiometricsTab';

export const PlayerProfilePage = () => {
  const { playerId } = useParams();
  const { data: playerProfile, isLoading } = usePlayersApi().useGetPlayerProfile(playerId);

  if (isLoading) return <p>Loading...</p>;

  return (
    <div className="container mt-4">
      <h2>{playerProfile.fullName}</h2>
      <Tabs defaultActiveKey="personal" id="player-profile-tabs" className="mb-3">
        <Tab eventKey="personal" title="Personal Info">
          <PersonalInfoTab profile={playerProfile.personalInfo} />
        </Tab>
        <Tab eventKey="skills" title="Football Skills">
          <FootballSkillsTab skills={playerProfile.skills} />
        </Tab>
        <Tab eventKey="biometrics" title="Biometrics">
          <BiometricsTab biometrics={playerProfile.biometrics} />
        </Tab>
      </Tabs>
    </div>
  );
};
```

## 5. Testing Strategy

The testing strategy will focus on the automatic age division assignment, as it's a critical piece of business logic that has direct implications on player eligibility and team rostering.

### 5.1. Backend Unit Test for Age Division Assignment

We will create a unit test for the `PlayerService` to verify that the `UpdatePlayerDivisionAsync` logic works correctly for various birth dates.

**Action:** Create a test file in `Diquis.Application.Tests`.

**File:** `Diquis.Application.Tests/Services/PlayerServiceTests.cs`
```csharp
using Diquis.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Diquis.Application.Tests.Services;

[TestFixture]
public class PlayerServiceTests
{
    private PlayerService _service;
    private Mock<ApplicationDbContext> _mockContext;
    private List<Division> _divisions;

    [SetUp]
    public void Setup()
    {
        _divisions = new List<Division>
        {
            new() { Id = Guid.NewGuid(), Name = "Under-13", MinAge = 12, MaxAge = 13, TenantId = "test" },
            new() { Id = Guid.NewGuid(), Name = "Under-15", MinAge = 14, MaxAge = 15, TenantId = "test" }
        };
        
        // Setup mock DbContext to return the divisions list
        _mockContext = new Mock<ApplicationDbContext>();
        _mockContext.Setup(c => c.Divisions).Returns(DbSetMock.Create(_divisions));

        // Instantiate the service with the mock context
        _service = new PlayerService(_mockContext.Object, /* other mocks */);
    }

    [Test]
    public async Task UpdatePlayerDivisionAsync_WhenPlayerIs12_ShouldAssignToU13Division()
    {
        // Arrange
        // We use a fixed "today" to make the test deterministic.
        var today = new DateOnly(2024, 8, 1); 
        var player = new ApplicationUser 
        {
            // This DOB makes the player 12 years old as of 'today'
            DateOfBirth = today.AddYears(-13).AddDays(1) 
        };

        // Act
        // Access the private method for testing purposes using a private accessor or by refactoring.
        // For this example, we assume it can be invoked for the test.
        await _service.UpdatePlayerDivisionAsync(player, today); // Pass 'today' to the method

        // Assert
        var u13Division = _divisions.First(d => d.Name == "Under-13");
        player.DivisionId.Should().Be(u13Division.Id);
    }

    [Test]
    public async Task UpdatePlayerDivisionAsync_WhenPlayerIs16_ShouldBeUncategorized()
    {
        // Arrange
        var today = new DateOnly(2024, 8, 1);
        var player = new ApplicationUser { DateOfBirth = today.AddYears(-16) };

        // Act
        await _service.UpdatePlayerDivisionAsync(player, today);

        // Assert
        player.DivisionId.Should().BeNull();
    }
}
```
This test suite ensures that the core business logic for player categorization is accurate and reliable.
