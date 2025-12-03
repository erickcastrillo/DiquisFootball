# Technical Implementation Guide: Player Management

This document provides a detailed technical guide for implementing the features outlined in the "Player Management" Functional Requirement Specification (FRS).

## 1. Architectural Analysis

### Domain Entities

This module extends the core `ApplicationUser` entity and introduces several new entities to capture the full scope of a player's profile and the academy's structure.

1.  **`ApplicationUser`** (Extend existing entity in `Diquis.Domain`):
    *   The "Player" is not a new entity, but rather an `ApplicationUser` with the `Player` role.
    *   **New Properties:**
        *   `ParentId` (Guid, nullable, FK to another `ApplicationUser`): Creates a direct link from a player to their parent/guardian.
        *   `PrimaryPositionId` (Guid, nullable, FK to `Position`): The player's main field position.
        *   `DivisionId` (Guid, nullable, FK to `Division`): The calculated age category for the player.
        *   `DateOfBirth` (DateOnly, required).
        *   `ProfilePictureUrl` (string, nullable).

2.  **`PlayerBiometric`** (New entity, in `Diquis.Domain`): A one-to-one extension of the player's profile for storing physical data.
    *   `PlayerId` (Guid, required, FK to `ApplicationUser`).
    *   `HeightCm` (decimal).
    *   `WeightKg` (decimal).
    *   `RecordedDate` (DateTime).

3.  **`Skill`** (New entity, in `Diquis.Domain`): A lookup table for skills defined by the academy.
    *   `Name` (string, required, e.g., "Dribbling").
    *   `Category` (string, e.g., "Technical," "Mental").

4.  **`PlayerSkill`** (New entity, in `Diquis.Domain`): A linking table for the many-to-many relationship between players and skills, which stores the coach's rating.
    *   `PlayerId` (Guid, required, FK to `ApplicationUser`).
    *   `SkillId` (Guid, required, FK to `Skill`).
    *   `Rating` (int, required, e.g., 1-10).
    *   `LastAssessed` (DateTime).

5.  **`Position`** (New entity, in `Diquis.Domain`): A lookup table for field positions defined by the academy.
    *   `Name` (string, required, e.g., "Centre-Back").
    *   `Abbreviation` (string, e.g., "CB").

6.  **`Division`** (New entity, in `Diquis.Domain`): Defines the age categories for the academy.
    *   `Name` (string, required, e.g., "Under-13s").
    *   `MinAge` (int, required).
    *   `MaxAge` (int, required).

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `PlayerBiometric`: A player's biometric data is exclusive to the academy.
    -   `Skill`: Each academy defines its own skill catalog.
    -   `PlayerSkill`: Player ratings are exclusive to the academy.
    -   `Position`: Each academy defines its own list of positions.
    -   `Division`: Each academy defines its own age categories.
-   The `ApplicationUser` entity is already correctly scoped by the boilerplate's architecture.

### Permissions & Authorization

| FRS Role | Policy Name | Permissions Claim |
| :--- | :--- | :--- |
| `academy_owner` | `IsAcademyOwner` | `permission:players.manage` |
| `director_of_football`| `IsDirector` | `permission:players.manage` |
| `coach` | `IsCoach` | `permission:players.manage.team` |
| `parent` | `IsParent` | `permission:players.update.personal` |
| `player` | `IsPlayer` | `permission:players.read.self` |

**Note:** The claims for `coach` and `parent` will require runtime validation to ensure they are only acting upon players on their team or their own children, respectively.

## 2. Scaffolding Strategy (CLI)

Execute these commands from the `Diquis.Application/Services` directory.

```bash
# Service for managing users with the "Player" role and their related data
dotnet new nano-service -s Player -p Players -ap Diquis
dotnet new nano-controller -s Player -p Players -ap Diquis

# Service for academy-defined Skills
dotnet new nano-service -s Skill -p Skills -ap Diquis
dotnet new nano-controller -s Skill -p Skills -ap Diquis

# Service for academy-defined Positions
dotnet new nano-service -s Position -p Positions -ap Diquis
dotnet new nano-controller -s Position -p Positions -ap Diquis

# Service for academy-defined Divisions (age categories)
dotnet new nano-service -s Division -p Divisions -ap Diquis
dotnet new nano-controller -s Division -p Divisions -ap Diquis
```

## 3. Implementation Plan (Agile Breakdown)

### User Story: Player Registration
**As a** `director_of_football` or `coach`, **I need to** register new players, **so that** they can be part of the academy.

**Technical Tasks:**
1.  **Domain:** Extend `ApplicationUser` with `ParentId`, `PrimaryPositionId`, `DivisionId`, `DateOfBirth`.
2.  **Persistence:** Generate a migration to update the `AspNetUsers` table in the `BaseDbContext`.
    -   *Migration Command:* `add-migration -Context BaseDbContext -o Persistence/Migrations/BaseDb AddPlayerFieldsToUser`
3.  **Application (DTOs):** Create `RegisterPlayerRequest` containing all personal info and parent details.
4.  **Application (Service):** In `PlayerService.cs`, create `RegisterPlayerAsync(RegisterPlayerRequest request)`.
    -   **Crucial Validation:** Before creating, check for existing users with the same name and DOB to prevent duplicates. Validate that `DateOfBirth` is in the past.
    -   If the parent does not exist, create a new `ApplicationUser` for them with the `Parent` role.
    -   Create the `ApplicationUser` for the player, assign the `Player` role, and set the `ParentId`.
    -   Call a private method `UpdatePlayerDivisionAsync(player)` to set the initial age category.
5.  **API:** The scaffolded `PlayersController` will have a `POST /api/players` endpoint that will be adapted for registration, secured by the `IsDirector` and `IsCoach` policies.
6.  **UI (React/Client):**
    -   Create a `src/pages/players/RegisterPlayerPage.tsx`.
    -   Implement a `<PlayerRegistrationForm />` component.

### User Story: Player Profile Management
**As a** `coach`, **I need to** view and update my players' football-specific information to track their development.

**Technical Tasks:**
1.  **Domain:** Create `PlayerBiometric`, `Skill`, `PlayerSkill`, and `Position` entities.
2.  **Persistence:** Add `DbSet`s for all new entities to `ApplicationDbContext` and create a migration.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddPlayerProfileEntities`
3.  **Application (DTOs):** Create a comprehensive `PlayerProfileDto` that aggregates data from `ApplicationUser`, `PlayerBiometric`, and `PlayerSkill`. Create `UpdateFootballProfileRequest` and `UpdatePersonalInfoRequest`.
4.  **Application (Service):** In `PlayerService.cs`:
    -   Implement `UpdatePlayerFootballInfoAsync(Guid playerId, UpdateFootballProfileRequest request)`.
        -   **Crucial Authorization:** This method must first verify the calling user is an authorized `coach` for the specified `playerId` or a `director_of_football`/`academy_owner`.
        -   It will update `PlayerBiometric` and `PlayerSkill` records.
    -   Implement `UpdatePlayerPersonalInfoAsync(Guid playerId, UpdatePersonalInfoRequest request)`.
        -   **Crucial Authorization:** This method must verify the calling user is the `parent` of the `playerId` or a higher-level admin.
5.  **API:**
    -   `GET /api/players/{id}`: Returns the full `PlayerProfileDto`.
    -   `PUT /api/players/{id}/football-info`: Secured endpoint for coaches/admins.
    -   `PUT /api/players/{id}/personal-info`: Secured endpoint for parents/admins.
6.  **UI (React/Client):**
    -   Create `src/pages/players/{id}/ProfilePage.tsx`.
    -   Conditionally render form fields as editable or read-only based on the logged-in user's role and their relationship to the player.
    -   Create a `PlayerSkillsMatrix.tsx` component to display and edit skill ratings.

### User Story: Automatic Age Category Validation
**As the** System, **I must** automatically validate and display a player's correct age category.

**Technical Tasks:**
1.  **Domain:** Create the `Division` entity as specified.
2.  **Persistence:** Add `DbSet<Division>` to `ApplicationDbContext` and create a migration.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddDivisionsEntity`
3.  **Application (Service):** In `PlayerService.cs`, create a private helper method: `UpdatePlayerDivisionAsync(ApplicationUser player)`.
    -   This method calculates `age = Today - player.DateOfBirth`.
    -   It then queries the `Divisions` table for the current tenant: `_context.Divisions.FirstOrDefaultAsync(d => age >= d.MinAge && age <= d.MaxAge)`.
    -   It updates `player.DivisionId` with the ID of the found division.
    -   This method must be called within `RegisterPlayerAsync` and after any update to a player's `DateOfBirth`.
4.  **API:** The scaffolded `DivisionsController` will be used by `academy_owner`s or `director_of_football`s to manage the age brackets for their academy.
5.  **UI (React/Client):**
    -   A `src/pages/academy/DivisionsPage.tsx` for admins to manage the divisions.
    -   The player's calculated division will be displayed as a read-only field on their profile page.

## 4. Code Specifications (Key Logic)

### `PlayerService.cs` - Authorization Check for Profile Update

```csharp
// Inside PlayerService.cs

public async Task UpdatePlayerFootballInfoAsync(Guid playerId, UpdateFootballProfileRequest request)
{
    var currentUserId = _currentUserService.UserId;
    var currentUserRoles = _currentUserService.Roles; // Assuming service provides roles

    var player = await _context.Users.FindAsync(playerId);
    if (player == null) throw new NotFoundException("Player not found.");

    bool isAuthorized = false;
    if (currentUserRoles.Contains("AcademyOwner") || currentUserRoles.Contains("DirectorOfFootball"))
    {
        isAuthorized = true;
    }
    else if (currentUserRoles.Contains("Coach"))
    {
        // isPlayerOnCoachTeam = await _context.TeamPlayers
        //    .AnyAsync(tp => tp.PlayerId == playerId && tp.Team.CoachId == currentUserId);
        // if (isPlayerOnCoachTeam) isAuthorized = true;
    }

    if (!isAuthorized)
    {
        throw new ForbiddenAccessException("You are not authorized to update this player's profile.");
    }
    
    // ... proceed with update logic for skills, biometrics, etc. ...
    await _context.SaveChangesAsync();
}
```

### `PlayerService.cs` - Automatic Division Calculation

```csharp
// Inside PlayerService.cs, to be called after creating/updating a player

private async Task UpdatePlayerDivisionAsync(ApplicationUser player)
{
    if (player.DateOfBirth == default) return;

    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    int age = today.Year - player.DateOfBirth.Year;
    if (player.DateOfBirth > today.AddYears(-age)) age--;

    var matchingDivision = await _context.Divisions
        .FirstOrDefaultAsync(d => age >= d.MinAge && age <= d.MaxAge);

    if (matchingDivision != null)
    {
        player.DivisionId = matchingDivision.Id;
    }
    else
    {
        player.DivisionId = null; // Uncategorized
    }
}
```

### `PlayerService.cs` - Registration Duplicate Check

```csharp
// Inside PlayerService.RegisterPlayerAsync, before creating the user

var potentialDuplicate = await _userManager.Users
    .AnyAsync(u => u.FirstName.ToLower() == request.FirstName.ToLower()
                && u.LastName.ToLower() == request.LastName.ToLower()
                && u.DateOfBirth == request.DateOfBirth);

if (potentialDuplicate)
{
    // In a real scenario, you might return a specific result object
    // instead of throwing an exception to allow the user to confirm.
    throw new ValidationException("A player with a similar name and the same date of birth already exists.");
}

// ... proceed with creating the ApplicationUser for the player and parent ...
```
