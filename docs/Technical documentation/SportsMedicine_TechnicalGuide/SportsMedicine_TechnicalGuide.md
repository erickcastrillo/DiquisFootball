# Technical Implementation Guide: Sports Medicine & Bio-Passport (Module 7)

This document provides a detailed technical guide for implementing the confidential "Sports Medicine & Bio-Passport" module as outlined in the corresponding Functional Requirement Specification (FRS).

## 1. Architectural Analysis

This module is designed to handle highly sensitive, encrypted medical data with strict, role-based access control.

### Domain Entities

1.  **`InjuryRecord`**: The central entity for the injury lifecycle.
    *   `PlayerId` (Guid, required, FK to `ApplicationUser`).
    *   `DateOfInjury` (DateOnly, required).
    *   `BodyPart` (string, encrypted).
    *   `InjuryType` (string, encrypted, e.g., "Sprain," "Fracture").
    *   `MechanismOfInjury` (string, encrypted, optional).
    *   `Diagnosis` (string, encrypted, required).
    *   `InjuryStatus` (enum: `Active`, `Closed`).
    *   `RtpStatus` (enum: `NotCleared`, `ClearedForNonContact`, `ClearedForFullTraining`).

2.  **`RehabilitationLog`**: Represents a single, time-stamped entry in an injury's recovery log.
    *   `InjuryRecordId` (Guid, required, FK to `InjuryRecord`).
    *   `LogDate` (DateTime, required).
    *   `Note` (string, encrypted, required).
    *   `LoggedBy` (Guid, FK to `ApplicationUser`).

3.  **`MedicalAttachment`**: Stores references to uploaded medical documents.
    *   `InjuryRecordId` (Guid, required, FK to `InjuryRecord`).
    *   `OriginalFilename` (string, required).
    *   `StorageUrl` (string, required, encrypted).
    *   `UploadedAt` (DateTime).

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `InjuryRecord`: All medical data is strictly owned by the academy tenant.
    -   `RehabilitationLog`: Inherits tenant scope from its parent `InjuryRecord`.
    -   `MedicalAttachment`: Inherits tenant scope from its parent `InjuryRecord`.

### Permissions & Authorization

| FRS Role | Policy Name | Permissions Claim | Implementation Detail |
| :--- | :--- | :--- | :--- |
| `director_of_football`| `IsMedicalStaff` | `permission:medical.fullaccess` | The primary role for managing medical records. |
| `academy_owner` | `IsAcademyOwner`| `permission:medical.statistics.read` | Grants access to anonymized, aggregate data only. |
| `coach` | `IsCoach` | `permission:medical.rtpstatus.read` | Can only read the `RtpStatus` enum for players on their team. |
| `parent` / `player`| `IsSelfOrParent`| (dynamic check) | Grants read-only access to their own/their child's full medical history. This will be enforced via a runtime check in the service layer, not a static claim. |

## 2. Scaffolding Strategy (CLI)

Execute these commands from the `Diquis.Application/Services` directory to create the base vertical slices.

```bash
# For managing the core injury records and their associated logs/attachments
dotnet new nano-service -s InjuryRecord -p InjuryRecords -ap Diquis
dotnet new nano-controller -s InjuryRecord -p InjuryRecords -ap Diquis
```
**Note:** The generated services and controllers will be heavily modified to implement the strict authorization, encryption, and business logic required.

## 3. Implementation Plan (Agile Breakdown)

### User Story: Injury Lifecycle Management
**As a** `director_of_football`, **I need to** log and manage the complete lifecycle of a player's injury, **so that** I can ensure a comprehensive and confidential medical history.

**Technical Tasks:**
1.  **Domain:** Create `InjuryRecord`, `RehabilitationLog`, and `MedicalAttachment` entities in `Diquis.Domain`. Ensure all implement `BaseEntity` and `IMustHaveTenant`. Mark sensitive string fields for encryption.
2.  **Persistence:** Add `DbSet`s for all three entities to `ApplicationDbContext`. Create a migration.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddSportsMedicineEntities`
3.  **Infrastructure (Encryption):**
    -   Create an `IEncryptionService` interface in the Application layer.
    -   Implement an `AesEncryptionService` in the Infrastructure layer.
    -   In `ApplicationDbContext.OnModelCreating`, use the `EncryptedStringConverter` pattern (from the Security NFRs) to apply encryption to all designated properties in the new entities.
4.  **Infrastructure (Audit Trail):**
    -   Add the `[Auditable]` attribute to the `InjuryRecord` entity to ensure all changes are logged by the existing audit system.
5.  **Application (DTOs):**
    -   `InjuryRecordDetailsDto`: Contains all fields for `director_of_football` and `player`/`parent` views.
    -   `CreateInjuryRecordRequest`: DTO for the creation form.
    -   `AnonymizedInjuryStatDto`: Contains `InjuryType` and `Count`.
    -   `PlayerRtpStatusDto`: Contains only `PlayerId` and `RtpStatus` for the coach view.
6.  **Application (Service):** In `InjuryRecordService.cs`:
    -   `CreateInjuryRecordAsync`: Secure with `IsMedicalStaff` policy.
    -   `GetInjuryDetailsAsync(Guid injuryId)`:
        -   **Crucial Authorization:** First, fetch the record. Then, check if the `currentUser` is the `director_of_football` OR is the `player` associated with the record OR is the `parent` of the player. Throw `ForbiddenAccessException` if none match.
    -   `GetInjuryStatisticsAsync()`: Secure with `IsAcademyOwner` policy. Returns `List<AnonymizedInjuryStatDto>`.
7.  **API:** In `InjuryRecordsController.cs`:
    -   Secure all CUD endpoints with `IsMedicalStaff` policy.
    -   Secure the `GET /{id}` endpoint to allow general authenticated access, as the service layer handles the fine-grained authorization.
    -   Create a new `GET /statistics` endpoint, secured with `IsAcademyOwner`.
8.  **UI (React/Client):**
    -   `PlayerProfilePage.tsx`: Add a new "Medical History" tab.
    -   `<InjuryList />`: Fetches and displays a player's injuries. An "Add Injury" button is visible only to `director_of_football`.
    -   `<InjuryForm />`: The modal for creating/editing an injury record.

### User Story: Rehabilitation & Return-to-Play (RTP) Protocol
**As a** `director_of_football`, **I need to** set a player's 'Return-to-Play' status and document their rehab progress.

**Technical Tasks:**
1.  **Application (DTOs):** `UpdateRtpStatusRequest`, `AddRehabLogRequest`.
2.  **Application (Service):** In `InjuryRecordService.cs`:
    -   `UpdateRtpStatusAsync(Guid injuryId, UpdateRtpStatusRequest request)`: Secure with `IsMedicalStaff`. Updates the `RtpStatus` on the `InjuryRecord`.
    -   `AddRehabilitationLogAsync(Guid injuryId, AddRehabLogRequest request)`: Secure with `IsMedicalStaff`. Creates a new `RehabilitationLog` entry linked to the injury.
    -   `GetPlayerRtpStatusesForTeamAsync(Guid teamId)`:
        -   Secure with `IsCoach` policy.
        -   Fetches active players on the team, joins to their **latest active** `InjuryRecord`, and returns a `List<PlayerRtpStatusDto>`. If a player has no active injury, their status is implicitly "Cleared for Full Training".
3.  **API:** Add `PUT /{id}/rtp-status` and `POST /{id}/rehab-log` endpoints to `InjuryRecordsController`, both secured for `IsMedicalStaff`. Create a new endpoint in `TeamsController` or a similar controller for `GET /api/teams/{id}/roster-fitness`, secured for `IsCoach`.
4.  **UI (React/Client):**
    -   `<InjuryDetailsPage />`: Display the current RTP status and a chronological list of rehab log entries. Allow medical staff to update them.
    -   `<TeamRosterPage />`: For the `coach`, display a simple icon (e.g., a colored dot) next to each player's name based on their `RtpStatus`.

### User Story: Medical Bio-Passport
**As a** `player` or `parent`, **I need to** access a complete, read-only history of my injuries and treatments.

**Technical Tasks:**
1.  **Application (Service):** The existing `GetInjuryDetailsAsync` and a new `GetInjuriesForPlayerAsync(Guid playerId)` method will provide the necessary data. The service-layer authorization already ensures users can only access their own/their child's data.
2.  **API:** `GET /api/players/{playerId}/medical-history`: This endpoint will be used by the frontend to populate the Bio-Passport view. The service layer's internal checks are sufficient for security.
3.  **UI (React/Client):**
    -   The "Medical History" tab on the `PlayerProfilePage.tsx` serves as the Bio-Passport.
    -   When viewed by a player or parent, all fields are read-only.
    -   This view will list all `InjuryRecord`s and, for each, display the details and the associated `RehabilitationLog` entries and `MedicalAttachment` links.

## 4. Code Specifications (Key Logic)

### `ApplicationDbContext.OnModelCreating` - Applying Field-Level Encryption

```csharp
// In Diquis.Infrastructure/Persistence/Contexts/ApplicationDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // NOTE: Key should be loaded securely from config/Key Vault, not hardcoded.
    var encryptionService = new AesEncryptionService(_config["Security:DbEncryptionKey"]);
    var encryptedStringConverter = new EncryptedStringConverter(encryptionService);

    // Apply converter to all sensitive fields in the Sports Medicine module
    modelBuilder.Entity<InjuryRecord>().Property(p => p.BodyPart).HasConversion(encryptedStringConverter);
    modelBuilder.Entity<InjuryRecord>().Property(p => p.InjuryType).HasConversion(encryptedStringConverter);
    modelBuilder.Entity<InjuryRecord>().Property(p => p.Diagnosis).HasConversion(encryptedStringConverter);
    modelBuilder.Entity<InjuryRecord>().Property(p => p.MechanismOfInjury).HasConversion(encryptedStringConverter);
    
    modelBuilder.Entity<RehabilitationLog>().Property(p => p.Note).HasConversion(encryptedStringConverter);
    
    modelBuilder.Entity<MedicalAttachment>().Property(p => p.StorageUrl).HasConversion(encryptedStringConverter);
}
```

### `InjuryRecordService.cs` - Fine-Grained Authorization for Data Access

```csharp
// Inside InjuryRecordService.cs

public async Task<InjuryRecordDetailsDto> GetInjuryDetailsAsync(Guid injuryId)
{
    var injury = await _context.InjuryRecords
        .Include(i => i.RehabilitationLogs)
        .Include(i => i.Attachments)
        .FirstOrDefaultAsync(i => i.Id == injuryId);

    if (injury == null) throw new NotFoundException("Injury record not found.");

    // --- Authorization Check ---
    var currentUser = _currentUserService; // Injected service providing UserId and Roles
    var player = await _context.Users.FindAsync(injury.PlayerId);

    bool canAccess = 
        // 1. Is user a medical staff?
        currentUser.HasPermission("permission:medical.fullaccess") ||
        // 2. Is user the player themselves?
        currentUser.UserId == injury.PlayerId ||
        // 3. Is user the parent of the player?
        (player != null && currentUser.UserId == player.ParentId);

    if (!canAccess)
    {
        throw new ForbiddenAccessException("You are not authorized to view these medical details.");
    }
    
    return _mapper.Map<InjuryRecordDetailsDto>(injury);
}
```

### `InjuryRecordService.cs` - Preventing Match Roster Addition

While this logic belongs in the `TeamService` or a `MatchService`, it would consume data exposed by the `InjuryRecordService`.

```csharp
// In a hypothetical MatchService.cs
public async Task AddPlayerToMatchRosterAsync(Guid matchId, Guid playerId)
{
    // Fetch the latest active injury for the player
    var latestInjury = await _context.InjuryRecords
        .Where(i => i.PlayerId == playerId && i.InjuryStatus == InjuryStatus.Active)
        .OrderByDescending(i => i.DateOfInjury)
        .FirstOrDefaultAsync();

    if (latestInjury != null && latestInjury.RtpStatus != RtpStatus.ClearedForFullTraining)
    {
        throw new ValidationException($"Player cannot be added to roster. Return-to-Play status is: {latestInjury.RtpStatus}.");
    }

    // ... proceed to add player to roster ...
}
```