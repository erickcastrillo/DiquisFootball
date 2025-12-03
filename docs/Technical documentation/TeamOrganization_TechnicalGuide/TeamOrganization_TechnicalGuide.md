# Technical Implementation Guide: Team Organization (Module 3)

This document provides a detailed technical guide for implementing the features outlined in the "Team Organization" Functional Requirement Specification (FRS).

## 1. Architectural Analysis

This module introduces the core concepts of teams and their rosters, linking players to coaches and divisions.

### Domain Entities

1.  **`Team`**: A new entity representing a single football team.
    *   `Name` (string, required).
    *   `CoachId` (Guid, required, FK to `ApplicationUser`).
    *   `DivisionId` (Guid, required, FK to `Division`).

2.  **`TeamPlayer`**: A new linking entity to manage the many-to-many relationship between `Team` and `Player` (`ApplicationUser`).
    *   `TeamId` (Guid, required, FK to `Team`).
    *   `PlayerId` (Guid, required, FK to `ApplicationUser`).

3.  **`Division`** (Extend existing entity in `Diquis.Domain`): The existing `Division` entity from the Player Management module will be extended.
    *   **New Property:** `Gender` (enum: `Male`, `Female`, `CoEd`).

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `Team`: Teams are exclusive to an academy.
    -   `TeamPlayer`: The link between a player and a team is exclusive to an academy.
    -   `Division`: As established in the Player Management module, divisions are defined per-academy.

### Permissions & Authorization

| FRS Role | Policy Name | Permissions Claim |
| :--- | :--- | :--- |
| `academy_owner` | `IsAcademyOwner` | `permission:teams.fullaccess` |
| `director_of_football`| `IsDirector` | `permission:teams.manage` |
| `coach` | `IsCoach` | `permission:roster.manage.ownteam` |

**Note:** The `IsCoach` policy will require runtime validation to ensure a coach can only manage the roster of teams they are directly assigned to.

## 2. Scaffolding Strategy (CLI)

Execute these commands from the `Diquis.Application/Services` directory.

```bash
# Service for managing Teams
dotnet new nano-service -s Team -p Teams -ap Diquis
dotnet new nano-controller -s Team -p Teams -ap Diquis

# The Division service/controller already exists from the Player Management module.
# Roster management will be implemented as custom methods within the TeamService.
```

## 3. Implementation Plan (Agile Breakdown)

### User Story: Division Management
**As a** `director_of_football`, **I need to** manage divisions, defining their age constraints and gender, **so that** teams can be correctly categorized.

**Technical Tasks:**
1.  **Domain:** Add the `Gender` enum property to the existing `Division.cs` entity in `Diquis.Domain`.
2.  **Persistence:** Generate a new migration to add the `Gender` column to the `Divisions` table.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddGenderToDivision`
3.  **Application (DTOs):** Update `DivisionDto` and `CreateUpdateDivisionRequest` in `Diquis.Application/Services/DivisionService/DTOs` to include the new `Gender` property.
4.  **Application (Service):** Update the `Create` and `Update` methods in `DivisionService.cs` to handle the `Gender` property.
5.  **API:** The existing `DivisionsController` will automatically support the updated DTOs.
6.  **UI (React/Client):**
    -   Update the form in `src/pages/academy/DivisionsPage.tsx` to include a dropdown selector for `Gender`.

### User Story: Team Creation
**As a** `director_of_football`, **I need to** create new teams, assigning a name, coach, and division.

**Technical Tasks:**
1.  **Domain:** Create the `Team.cs` entity in `Diquis.Domain/Entities`. Ensure it inherits `BaseEntity` and implements `IMustHaveTenant`.
2.  **Persistence:** Add `DbSet<Team>` to `ApplicationDbContext` and create a migration.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddTeamEntity`
3.  **Application (DTOs):** In `Diquis.Application/Services/TeamService/DTOs`, define `TeamDto` and `CreateTeamRequest`.
4.  **Application (Service):** In `TeamService.cs`:
    -   Implement `CreateTeamAsync(CreateTeamRequest request)`.
    -   **Crucial Validation:** Before creating, verify `request.Name` is unique for the given `request.DivisionId`. Throw a `ValidationException` if a duplicate is found.
5.  **API:** Secure the scaffolded `TeamsController` endpoints. `POST`, `PUT`, `DELETE` are restricted to `IsAcademyOwner` and `IsDirector`. `GET` is available to all authenticated users.
6.  **UI (React/Client):**
    -   Create `src/pages/teams/TeamListPage.tsx` to display all academy teams.
    -   A "Create Team" button (visible to admins) will open a `<CreateTeamForm />` modal with dropdowns for selecting the `Coach` and `Division`.

### User Story: Team Roster Management
**As a** `coach`, **I need to** add and remove players from my team's roster to finalize my squad.

**Technical Tasks:**
1.  **Domain:** Create the `TeamPlayer.cs` linking entity.
2.  **Persistence:** Add `DbSet<TeamPlayer>` to `ApplicationDbContext` and update the migration.
3.  **Application (DTOs):** Create `RosterPlayerDto` (containing PlayerId, Name, Age, Position) and `UpdateRosterRequest` (`List<Guid> playerIdsToAdd`, `List<Guid> playerIdsToRemove`).
4.  **Application (Service):** In `TeamService.cs`, create new orchestration methods:
    -   `GetRosterAsync(Guid teamId)`: Returns a `List<RosterPlayerDto>` for the specified team.
    -   `GetEligiblePlayersAsync(Guid teamId)`:
        -   **Crucial Logic:** Fetches the team's division, calculates the valid birth date range based on the division's age constraints, and returns a list of players who are within that age range and are not currently on any other team roster.
    -   `UpdateRosterAsync(Guid teamId, UpdateRosterRequest request)`:
        -   **Crucial Authorization:** Verify the current user is the coach of this `teamId` or a director/owner.
        -   **Crucial Validation:** For each player in `playerIdsToAdd`, verify they are not already on another team.
        -   Perform the add/remove operations on the `TeamPlayer` table within a single transaction.
5.  **API:** In `TeamsController.cs`, add new endpoints:
    -   `GET /api/teams/{id}/roster`: Calls `GetRosterAsync`.
    -   `GET /api/teams/{id}/eligible-players`: Calls `GetEligiblePlayersAsync`. Secured for the assigned coach and admins.
    -   `PUT /api/teams/{id}/roster`: Calls `UpdateRosterAsync`. Secured for the assigned coach and admins.
6.  **UI (React/Client):**
    -   Create `src/pages/teams/{id}/RosterManagementPage.tsx`.
    -   Display two columns: "Eligible Players" and "Current Roster".
    -   Implement drag-and-drop functionality between the two lists to manage the roster.
    -   **TanStack Table Columns (Roster):** `Player Name`, `Age`, `Primary Position`, `Actions (Remove)`.

## 4. Code Specifications (Key Logic)

### `TeamService.cs` - Team Name Uniqueness Validation

```csharp
// Inside TeamService.CreateTeamAsync method

public async Task<Guid> CreateTeamAsync(CreateTeamRequest request)
{
    // Check for unique name within the same division
    var nameIsTaken = await _context.Teams
        .AnyAsync(t => t.DivisionId == request.DivisionId && t.Name.ToLower() == request.Name.ToLower());
        
    if (nameIsTaken)
    {
        throw new ValidationException($"A team named '{request.Name}' already exists in this division.");
    }

    // ... proceed to map DTO and create the Team entity ...
}
```

### `TeamService.cs` - Eligible Player Filtering Logic

```csharp
// Inside TeamService.cs

public async Task<List<RosterPlayerDto>> GetEligiblePlayersAsync(Guid teamId)
{
    var team = await _context.Teams.Include(t => t.Division).FirstOrDefaultAsync(t => t.Id == teamId);
    if (team == null) throw new NotFoundException("Team not found.");

    // Calculate the valid birth date range based on the division's age rules.
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var minBirthDate = today.AddYears(-(team.Division.MaxAge + 1)).AddDays(1);
    var maxBirthDate = today.AddYears(-team.Division.MinAge);

    // Get a list of all player IDs that are already on ANY team roster.
    var assignedPlayerIds = await _context.TeamPlayers
        .Select(tp => tp.PlayerId)
        .Distinct()
        .ToListAsync();

    // Find all players who meet the age criteria and are not already assigned.
    var eligiblePlayers = await _context.Users
        .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Player") // Is a player
                    && u.DateOfBirth >= minBirthDate 
                    && u.DateOfBirth <= maxBirthDate
                    && !assignedPlayerIds.Contains(u.Id))
        .ProjectTo<RosterPlayerDto>(_mapper.ConfigurationProvider) // Project to DTO
        .ToListAsync();

    return eligiblePlayers;
}
```

### `TeamService.cs` - Roster Update Authorization and Logic

```csharp
// Inside TeamService.cs

public async Task UpdateRosterAsync(Guid teamId, UpdateRosterRequest request)
{
    var team = await _context.Teams.FindAsync(teamId);
    if (team == null) throw new NotFoundException("Team not found.");

    // --- Authorization Check ---
    var currentUser = _currentUserService;
    bool isAuthorized = currentUser.HasPermission("permission:teams.fullaccess") || 
                        (currentUser.HasPermission("permission:roster.manage.ownteam") && team.CoachId == currentUser.UserId);

    if (!isAuthorized)
    {
        throw new ForbiddenAccessException("You are not authorized to manage this team's roster.");
    }
    
    // --- Business Logic in a Transaction ---
    await using var transaction = await _context.Database.BeginTransactionAsync();

    // Remove players
    if (request.PlayerIdsToRemove != null && request.PlayerIdsToRemove.Any())
    {
        await _context.TeamPlayers
            .Where(tp => tp.TeamId == teamId && request.PlayerIdsToRemove.Contains(tp.PlayerId))
            .ExecuteDeleteAsync();
    }

    // Add players (with validation)
    if (request.PlayerIdsToAdd != null && request.PlayerIdsToAdd.Any())
    {
        var alreadyAssigned = await _context.TeamPlayers
            .Where(tp => request.PlayerIdsToAdd.Contains(tp.PlayerId))
            .Select(tp => tp.PlayerId)
            .ToListAsync();
            
        if (alreadyAssigned.Any())
        {
            await transaction.RollbackAsync();
            throw new ValidationException($"One or more players are already assigned to another team. Player IDs: {string.Join(", ", alreadyAssigned)}");
        }
        
        var newRosterEntries = request.PlayerIdsToAdd.Select(playerId => new TeamPlayer { TeamId = teamId, PlayerId = playerId });
        await _context.TeamPlayers.AddRangeAsync(newRosterEntries);
    }
    
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
```