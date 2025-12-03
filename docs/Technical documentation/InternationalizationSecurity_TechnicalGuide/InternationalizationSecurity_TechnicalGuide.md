# Technical Implementation Guide: Internationalization & Security (NFRs)

This document provides a detailed technical guide for implementing the critical Non-Functional Requirements (NFRs) for Internationalization (Module 11) and Security (Module 12) as outlined in the FRS.

## 1. Architectural Analysis

### Domain Entities

1.  **`ApplicationUser`** (Extend existing entity in `Diquis.Domain`):
    *   Add `Locale` (string, nullable, e.g., "es-CR", "en-US"): Stores the user's preferred language and regional format settings.

2.  **`Translation`** (New entity, in `Diquis.Domain`): To store UI labels centrally.
    *   `Key` (string, required, e.g., "labels.playerProfile.medicalHistory").
    *   `LanguageCode` (string, required, e.g., "en", "es").
    *   `Value` (string, required, the translated text).
    *   This entity **MUST NOT** implement `IMustHaveTenant` as translations are global.

3.  **`AuditLog`** (New entity, in `Diquis.Domain`): For immutable security audit trails.
    *   `Timestamp` (DateTime, UTC).
    *   `UserId` (Guid, who performed the action).
    *   `SourceIpAddress` (string).
    *   `ActionType` (string, "CREATE", "UPDATE", "DELETE").
    *   `TableName` (string, the table affected).
    *   `RecordId` (string, the primary key of the affected record).
    *   `OldValue` (JSON string, blob of the old record state).
    *   `NewValue` (JSON string, blob of the new record state).
    *   This entity **MUST** implement `IMustHaveTenant` to keep audit logs scoped to the academy.

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `AuditLog`: Each academy's audit trail is strictly isolated.
-   **Shared/Global:**
    -   `Translation`: Language packs are a system-wide resource, available to all tenants.
    -   The `Locale` property on `ApplicationUser` is tenant-scoped by virtue of the user belonging to a tenant.

### Permissions & Authorization

These modules introduce system-level behaviors rather than new user-facing roles. The key authorization considerations are:

| FRS Requirement | Actor | Policy/Claim Needed | Implementation Detail |
| :--- | :--- | :--- | :--- |
| Manage Translations | `super_user` | `permission:translations.manage` | Secures the `TranslationsController` endpoints. |
| Anonymize Player | `academy_owner` | `permission:player.anonymize` | Secures the `AnonymizePlayer` endpoint. Requires re-authentication. |

## 2. Scaffolding Strategy (CLI)

Standard CRUD scaffolding is only partially applicable.

1.  **Translation Management (for Admins):**
    *Execute from `Diquis.Application/Services`*
    ```bash
    dotnet new nano-service -s Translation -p Translations -ap Diquis
    dotnet new nano-controller -s Translation -p Translations -ap Diquis
    ```

2.  **Manual Implementation:**
    -   **Audit Logging:** Will be implemented via a `DbContext` override, not a dedicated service.
    -   **GDPR Anonymization:** Will be a new method within the existing `PlayerService` (assuming one exists).
    -   **Locale Management:** Will be part of the existing `IdentityService` or a `UserProfileService`.

## 3. Implementation Plan (Agile Breakdown)

### User Story: User Locale Preference
**As a** Coach in Costa Rica, **I want** my UI to be in Spanish and dates formatted as DD/MM/YYYY, **so that** the platform feels natural to me.

**Technical Tasks:**
1.  **Domain:** Add the `string Locale` property to `Diquis.Domain/Entities/Identity/ApplicationUser.cs`.
2.  **Persistence:** Generate a migration to add the `Locale` column to the `AspNetUsers` table.
    -   *Migration Command:* `add-migration -Context BaseDbContext -o Persistence/Migrations/BaseDb AddLocaleToUser`
3.  **Application (Service):**
    -   In `IdentityService` or a `UserProfileService`, add a method `UpdateUserLocaleAsync(string userId, string newLocale)`.
    -   On user login (`TokenService`), include the user's `Locale` as a claim in the JWT.
4.  **API:**
    -   Expose a `PUT /api/identity/profile/locale` endpoint for a user to update their preference.
5.  **UI (React/Client):**
    -   The client application will read the `locale` claim from the JWT upon login.
    -   This value will be used to initialize the i18n library (e.g., `i18next`) and date formatting library (e.g., `date-fns-tz`) for the entire user session.

### User Story: Centralized Translation Management
**As a** System Administrator, **I need** to manage all language translations centrally, **so that** we can add languages without code deployments.

**Technical Tasks:**
1.  **Domain:** Create the `Translation.cs` entity as specified above.
2.  **Persistence:** Add `DbSet<Translation>` to `BaseDbContext` (as it's global) and generate a migration.
    -   *Migration Command:* `add-migration -Context BaseDbContext -o Persistence/Migrations/BaseDb AddTranslationsEntity`
3.  **Application (Service):** The scaffolded `TranslationService` will provide CRUD operations. Add a new method `GetTranslationsAsDictionaryAsync(string languageCode)` that returns a `Dictionary<string, string>`. Implement caching (e.g., Redis) here for performance.
4.  **API:** The scaffolded `TranslationsController` will be secured for `super_user`. Add a public, anonymous-optional endpoint `GET /api/translations/{languageCode}` that serves the dictionary to the front end.
5.  **UI (React/Client):** The i18n library will fetch its translation file from the new public endpoint (e.g., `/api/translations/es`). The library should be configured to use `en` as the fallback language.

### User Story: Immutable Audit Trails
**As an** Academy Owner, **I need to** know exactly who changed a player's contract salary, **so that** I have a full audit trail.

**Technical Tasks:**
1.  **Domain:** Create the `AuditLog.cs` entity as specified above. Mark properties that will hold JSON with `[Column(TypeName = "jsonb")]` for PostgreSQL.
2.  **Persistence:** Add `DbSet<AuditLog>` to `ApplicationDbContext` (as logs are tenant-specific). Generate a migration.
3.  **Infrastructure:** In `Diquis.Infrastructure/Persistence/Contexts/ApplicationDbContext.cs`, override the `SaveChangesAsync` method.
    -   This override will inspect the `ChangeTracker` for entities marked with a custom `[Auditable]` attribute.
    -   For each changed entity, it will create an `AuditLog` entry, serialize the old and new values to JSON, and save it within the same transaction.
4.  **UI (React/Client):** Create a new page `src/pages/auditing/AuditTrail.tsx` for `academy_owner`s to view and filter the logs.

### User Story: GDPR "Right to be Forgotten"
**As a** Parent, **I am exercising my right** to have my child's personal data permanently deleted from your systems.

**Technical Tasks:**
1.  **Application (Service):** In the service responsible for players (e.g., `PlayerService`), create a new method `AnonymizePlayerAsync(Guid playerId)`.
2.  **Logic:**
    -   Fetch the player record.
    -   Overwrite PII fields: `Name = "Former Player"`, `Email = null`, `DateOfBirth = null`, etc.
    -   Hard delete associated highly sensitive records (e.g., from the `Medical` module).
    -   **Do not** delete non-PII statistical records.
    -   Save the changes. This action will be automatically logged by the new audit system.
3.  **API:** Create a `POST /api/players/{id}/anonymize` endpoint, secured by the `IsAcademyOwner` policy, that calls this service method.

## 4. Code Specifications (Key Logic)

### `ApplicationDbContext.cs` - Automated Audit Log Generation

```csharp
// In Diquis.Infrastructure/Persistence/Contexts/ApplicationDbContext.cs
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
{
    var auditEntries = OnBeforeSaveChanges();
    var result = await base.SaveChangesAsync(cancellationToken);
    await OnAfterSaveChanges(auditEntries);
    return result;
}

private List<AuditEntry> OnBeforeSaveChanges()
{
    ChangeTracker.DetectChanges();
    var entries = new List<AuditEntry>();

    foreach (var entry in ChangeTracker.Entries())
    {
        if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
            continue;
        
        // Ensure the entity is marked as [Auditable]
        if (entry.Entity.GetType().GetCustomAttribute<AuditableAttribute>() == null)
            continue;

        var auditEntry = new AuditEntry(entry)
        {
            TableName = entry.Metadata.GetTableName(),
            // UserId and IP Address should be injected from a service like ICurrentUserService
        };
        entries.Add(auditEntry);

        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary) continue;

            string propertyName = property.Metadata.Name;
            if (property.Metadata.IsPrimaryKey())
            {
                auditEntry.KeyValues[propertyName] = property.CurrentValue;
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.AuditType = "CREATE";
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    auditEntry.AuditType = "DELETE";
                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                    break;

                case EntityState.Modified:
                    if (property.IsModified)
                    {
                        auditEntry.AuditType = "UPDATE";
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                    }
                    break;
            }
        }
    }
    return entries;
}

private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
{
    if (auditEntries == null || auditEntries.Count == 0) return;

    foreach (var auditEntry in auditEntries)
    {
        var log = new AuditLog
        {
            UserId = auditEntry.UserId,
            ActionType = auditEntry.AuditType,
            TableName = auditEntry.TableName,
            Timestamp = DateTime.UtcNow,
            RecordId = JsonConvert.SerializeObject(auditEntry.KeyValues),
            OldValue = auditEntry.OldValues.Count == 0 ? null : JsonConvert.SerializeObject(auditEntry.OldValues),
            NewValue = auditEntry.NewValues.Count == 0 ? null : JsonConvert.SerializeObject(auditEntry.NewValues)
            // TenantId is set automatically
        };
        await AuditLogs.AddAsync(log);
    }
    await base.SaveChangesAsync(); // Save the audit logs themselves
}

// Helper class AuditEntry and [Auditable] attribute would also be defined.
```

### `PlayerService.cs` - GDPR Anonymization Logic (Pseudo-code)

```csharp
// In a service like Diquis.Application/Services/PlayerService/PlayerService.cs

public async Task AnonymizePlayerAsync(Guid playerId)
{
    var player = await _context.Users
        .Include(u => u.MedicalRecords) // Assuming relation exists
        .FirstOrDefaultAsync(u => u.Id == playerId);

    if (player == null) throw new NotFoundException("Player not found.");

    // 1. Hard Anonymize PII
    player.FirstName = "Former";
    player.LastName = $"Player-{player.Id.ToString().Substring(0, 8)}";
    player.Email = null;
    player.PhoneNumber = null;
    player.DateOfBirth = null;
    // Overwrite any other PII fields

    // 2. Hard Delete Highly Sensitive Data
    if(player.MedicalRecords.Any())
    {
        _context.MedicalRecords.RemoveRange(player.MedicalRecords);
    }

    // This update will be logged by the audit system automatically.
    await _context.SaveChangesAsync();
}
```

### Field-Level Encryption using Value Converters

For the **Sports Medicine** module, an `EncryptedStringConverter` would be created in the Infrastructure layer.

```csharp
// In Diquis.Infrastructure/Persistence/Extensions/EncryptedStringConverter.cs
public class EncryptedStringConverter : ValueConverter<string, string>
{
    public EncryptedStringConverter(IEncryptionService encryptionService, ConverterMappingHints mappingHints = null)
        : base(
            v => encryptionService.Encrypt(v), // Encrypt on write
            v => encryptionService.Decrypt(v), // Decrypt on read
            mappingHints)
    {
    }
}

// Then, in ApplicationDbContext.cs OnModelCreating:
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var encryptionService = new AesEncryptionService("Your-Key-From-Config"); // Key should come from a secure source
    
    modelBuilder.Entity<MedicalRecord>()
        .Property(e => e.Notes)
        .HasConversion(new EncryptedStringConverter(encryptionService));

    modelBuilder.Entity<MedicalRecord>()
        .Property(e => e.Diagnosis)
        .HasConversion(new EncryptedStringConverter(encryptionService));
}
```
