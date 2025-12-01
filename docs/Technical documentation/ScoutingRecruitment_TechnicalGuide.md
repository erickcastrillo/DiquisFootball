# Technical Implementation Guide: Scouting & Recruitment

This document provides a detailed technical guide for implementing the "Scouting & Recruitment" module (Module 8). It covers the management of external trialists, standardized evaluations, and the process of promoting a prospect to a full academy player.

## 1. Architectural Analysis

This module introduces a parallel system for managing players who are not yet part of the academy. The core challenge is tracking these "trialists" and their evaluations, and then providing a seamless workflow to convert them into full `ApplicationUser` players while preserving their history.

### Domain Entities

1.  **`Trialist`** (New entity, in `Diquis.Domain`): Represents a prospective player being evaluated by the academy. This entity is kept separate from the main `ApplicationUser` table to avoid cluttering the primary roster.
    *   `Name` (string, required).
    *   `DateOfBirth` (DateOnly, required).
    *   `ContactInfo` (string, optional).
    *   `PreviousClub` (string, optional).
    *   `Status` (enum: `Active`, `Archived`, `Promoted`).
    *   `PromotedToPlayerId` (Guid, nullable, FK to `ApplicationUser`): A link to the full player profile created after conversion.

2.  **`ShadowSquad`** (New entity, in `Diquis.Domain`): A logical grouping for trialists, used for organizational purposes (e.g., "U17 Fall Trialists").
    *   `Name` (string, required).
    *   `Description` (string, optional).

3.  **`ShadowSquadTrialist`** (New linking entity, in `Diquis.Domain`): Manages the many-to-many relationship between `ShadowSquad` and `Trialist`.
    *   `ShadowSquadId` (Guid, FK to `ShadowSquad`).
    *   `TrialistId` (Guid, FK to `Trialist`).

4.  **`EvaluationCard`** (New entity, in `Diquis.Domain`): A standardized scorecard for a trialist.
    *   `TrialistId` (Guid, required, FK to `Trialist`).
    *   `ScoutId` (Guid, required, FK to `ApplicationUser`): The coach or director who performed the evaluation.
    *   `EvaluationDate` (DateTime, required).
    *   `TechnicalRating` (int, 1-10).
    *   `TacticalRating` (int, 1-10).
    *   `PhysicalRating` (int, 1-10).
    *   `PsychologicalRating` (int, 1-10).
    *   `SummaryNotes` (string, required, max length).
    *   `PlayerId` (Guid, nullable, FK to `ApplicationUser`): After a trialist is promoted, this field is back-filled to link the historical evaluation to the official player profile.

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `Trialist`: Prospects are scouted by and for a specific academy.
    -   `ShadowSquad`: These are internal groupings within an academy.
    -   `ShadowSquadTrialist`: The link is academy-specific.
    -   `EvaluationCard`: Evaluations are proprietary to the academy that performs them.

### Permissions & Authorization

| FRS Role | Policy Name | Permissions Claim | Implementation Detail |
| :--- | :--- | :--- | :--- |
| `director_of_football`| `IsDirector` | `permission:scouting.admin` | Full control over trialists, squads, and evaluations. Can execute promotions. |
| `coach` | `IsCoach` | `permission:scouting.evaluate` | Can create trialists and their own evaluations. Can only view evaluations for squads they are assigned to. Cannot manage squads or promote trialists. |

## 2. Scaffolding Strategy (CLI)

Execute these commands from the `Diquis.Application/Services` directory to create the vertical slices for the new entities.

```bash
# For managing trialist profiles
dotnet new nano-service -s Trialist -p Trialists -ap Diquis
dotnet new nano-controller -s Trialist -p Trialists -ap Diquis

# For managing shadow squads
dotnet new nano-service -s ShadowSquad -p ShadowSquads -ap Diquis
dotnet new nano-controller -s ShadowSquad -p ShadowSquads -ap Diquis

# For managing evaluation cards
dotnet new nano-service -s EvaluationCard -p EvaluationCards -ap Diquis
dotnet new nano-controller -s EvaluationCard -p EvaluationCards -ap Diquis
```

## 3. Implementation Plan (Agile Breakdown)

### User Story: Trialist Management & "Shadow Squads"
**As a** `director_of_football`, **I need to** create profiles for trialists and organize them into "Shadow Squads", **so that** we can efficiently manage external talent.

**Technical Tasks:**
1.  **Domain:** Create `Trialist`, `ShadowSquad`, and `ShadowSquadTrialist` entities.
2.  **Persistence:** Add `DbSet`s for all three entities to `ApplicationDbContext` and create a migration.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddScoutingEntities`
3.  **Application (DTOs):** Create DTOs for `Trialist` and `ShadowSquad` (e.g., `TrialistDto`, `CreateTrialistRequest`, `ShadowSquadDto`).
4.  **Application (Service):**
    -   In `TrialistService.cs`, implement CRUD operations. The `Create` method must check for duplicates against both the `Trialists` table and the `AspNetUsers` table.
    -   In `ShadowSquadService.cs`, implement CRUD for squads and add methods `AddTrialistToSquadAsync(Guid squadId, Guid trialistId)` and `RemoveTrialistFromSquadAsync(...)` to manage the linking table.
5.  **API:** Secure the generated `TrialistsController` and `ShadowSquadsController` endpoints with the appropriate policies (`IsDirector`, `IsCoach`).
6.  **UI (React/Client):**
    -   Create `src/pages/scouting/ScoutingDashboardPage.tsx`.
    -   This page will feature two main components: `<TrialistList />` and `<ShadowSquadList />`.
    -   Use TanStack Table for both lists. **Trialist Table Columns:** `Name`, `Age`, `Previous Club`, `Status`, `Actions`.
    -   Clicking on a shadow squad will filter the trialist list.

### User Story: Standardized Talent Evaluation Cards
**As a** `coach`, **I must** use a standardized evaluation card to rate a trialist's abilities, **so that** all prospects are judged by the same objective criteria.

**Technical Tasks:**
1.  **Domain:** Create the `EvaluationCard` entity as defined above. Add the nullable `PlayerId` for the future promotion step.
2.  **Persistence:** Add `DbSet<EvaluationCard>` to `ApplicationDbContext` and update the migration.
3.  **Application (DTOs):** Create `EvaluationCardDto` and `CreateEvaluationCardRequest`.
4.  **Application (Service):** In `EvaluationCardService.cs`:
    -   **Crucial Validation:** In `CreateEvaluationCardAsync`, add validation to ensure all four rating properties are between 1 and 10, and that `SummaryNotes` is not null or whitespace.
    -   The `ScoutId` should be set automatically from the `_currentUserService`.
    -   In `GetByTrialistIdAsync`, implement authorization logic to ensure a `coach` can only view cards for trialists in squads they are assigned to.
5.  **API:** Secure `EvaluationCardsController` endpoints. `POST` is available to `coach` and `director`. `DELETE` is only available to `director`.
6.  **UI (React/Client):**
    -   On the `TrialistProfilePage.tsx`, add an "Add Evaluation" button.
    -   This button opens a `<EvaluationCardForm />` modal containing sliders (1-10) for each of the 4 rating categories and a required text area for the summary.
    -   The profile page will also display a list of past `<EvaluationCardItem />` components.

### User Story: Convert Trialist to Full Academy Player
**As a** `director_of_football`, **I need** a simple process to convert a trialist into a full academy player.

**Technical Tasks:**
1.  **Application (Service):** This is a new, high-level orchestration method. It can be placed in a `ScoutingOrchestrationService.cs` or within the `TrialistService.cs`.
    -   Implement `PromoteTrialistToPlayerAsync(Guid trialistId, PromoteTrialistRequest request)`.
2.  **Logic (Crucial):**
    1.  Start a `DbContext` database transaction.
    2.  Fetch the `Trialist` record. Validate its `Status` is `Active`.
    3.  Call the existing `PlayerService.RegisterPlayerAsync` method, passing in the trialist's data combined with any new data from the `PromoteTrialistRequest` (e.g., parent info).
    4.  Receive the `newPlayerId` (the `ApplicationUser.Id`) from the `PlayerService`.
    5.  Update the `Trialist` record: `Status = TrialistStatus.Promoted`, `PromotedToPlayerId = newPlayerId`.
    6.  Find all `EvaluationCard`s where `TrialistId == trialistId`.
    7.  Loop through the cards and update their `PlayerId` field to `newPlayerId`.
    8.  Commit the transaction. If any step fails, the transaction will roll back all changes.
3.  **API:** In `TrialistsController.cs` (or a new `ScoutingController`), create a new endpoint: `POST /api/trialists/{id}/promote`.
    -   This endpoint is secured with the `IsDirector` policy.
4.  **UI (React/Client):**
    -   Add a "Promote to Academy" button on the `TrialistProfilePage.tsx`, visible only to directors.
    -   This opens the standard `<PlayerRegistrationForm />` component, but with its initial state pre-populated from the trialist's data.
    -   The `PlayerProfilePage.tsx` must be updated to include a "Scouting History" tab, which will fetch and display any `EvaluationCard` records linked via the `PlayerId`.

## 4. Code Specifications (Key Logic)

### `TrialistService.cs` - Promotion Orchestration Logic

```csharp
// Inside TrialistService.cs or a new ScoutingOrchestrationService.cs

public async Task<Guid> PromoteTrialistToPlayerAsync(Guid trialistId, PromoteTrialistRequest promotionRequest)
{
    // A single transaction ensures this is an atomic operation
    await using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        var trialist = await _context.Trialists.FindAsync(trialistId);
        if (trialist == null || trialist.Status != TrialistStatus.Active)
        {
            throw new InvalidOperationException("Trialist is not active or does not exist.");
        }

        // 1. Create the full player profile by calling the existing player service
        var registerPlayerRequest = new RegisterPlayerRequest
        {
            FullName = trialist.Name,
            DateOfBirth = trialist.DateOfBirth,
            // Map other fields from trialist and promotionRequest...
            ParentEmail = promotionRequest.ParentEmail
        };
        var newPlayerId = await _playerService.RegisterPlayerAsync(registerPlayerRequest);

        // 2. Update the original trialist to an archived/promoted state
        trialist.Status = TrialistStatus.Promoted;
        trialist.PromotedToPlayerId = newPlayerId;

        // 3. Back-fill the PlayerId on all historical evaluation cards
        var evaluations = await _context.EvaluationCards
            .Where(e => e.TrialistId == trialistId)
            .ToListAsync();
            
        foreach (var card in evaluations)
        {
            card.PlayerId = newPlayerId;
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return newPlayerId;
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw; // Re-throw the exception to be handled by global exception middleware
    }
}
```

### `EvaluationCardService.cs` - Creation Validation

```csharp
// Inside EvaluationCardService.cs

public async Task<Guid> CreateEvaluationCardAsync(CreateEvaluationCardRequest request)
{
    // FluentValidation is preferred, but showing manual validation for clarity
    if (request.TechnicalRating < 1 || request.TechnicalRating > 10 ||
        request.TacticalRating < 1 || request.TacticalRating > 10 ||
        request.PhysicalRating < 1 || request.PhysicalRating > 10 ||
        request.PsychologicalRating < 1 || request.PsychologicalRating > 10)
    {
        throw new ValidationException("All rating fields must be between 1 and 10.");
    }

    if (string.IsNullOrWhiteSpace(request.SummaryNotes))
    {
        throw new ValidationException("Summary Notes are required.");
    }
    
    var card = _mapper.Map<EvaluationCard>(request);
    card.ScoutId = _currentUserService.UserId; // Set the evaluator automatically
    card.EvaluationDate = DateTime.UtcNow;

    _context.EvaluationCards.Add(card);
    await _context.SaveChangesAsync();

    return card.Id;
}
```
