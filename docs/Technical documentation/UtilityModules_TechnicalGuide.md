# Technical Implementation Guide: Utility & Platform Services

This document provides a detailed technical guide for implementing the core utility modules as outlined in the FRS:
- **Module 13:** Communication & Notification Engine
- **Module 14:** Data Portability & Migration Tools
- **Module 15:** Digital Signatures & Documents

---

## Module 13: 🔔 Communication & Notification Engine

This module establishes a centralized service for handling all user-facing alerts. It is a cross-cutting concern, designed to be called by other application services.

### 1. Architectural Analysis

#### Domain Entities

1.  **`Notification`**: Represents a single, discrete notification to be displayed to a user.
    *   `UserId` (Guid, required, FK to `ApplicationUser`).
    *   `Message` (string, required).
    *   `EventType` (string, required, e.g., "PlayerInjuryStatusChanged").
    *   `IsRead` (bool, default: false).
    *   `Criticality` (enum: `Informational`, `Warning`, `Critical`).
    *   `DeepLink` (string, optional): A client-side route to navigate to on-click (e.g., `/players/{id}/medical`).

2.  **`UserNotificationPreference`**: A one-to-one entity linked to `ApplicationUser` storing their notification choices.
    *   `UserId` (Guid, required, PK/FK to `ApplicationUser`).
    *   `Preferences` (JSON, string): A serialized dictionary mapping `EventType` to preferred channels and digest settings (e.g., `{"PlayerProfileUpdated": {"channels": ["Email", "InApp"], "digest": true}}`).

#### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `Notification`: All notifications are tenant-specific.
    -   `UserNotificationPreference`: User preferences are tenant-specific.

#### Permissions & Authorization
- **Broadcasts:** Sending broadcast messages will be role-restricted at the calling service level (e.g., an `Academy Admin` calling `TrainingSessionService.CancelSessionAsync`).
- **Preferences:** Users can only update their own preferences.

### 2. Scaffolding Strategy (CLI)

Standard CRUD scaffolding is **not suitable** for this module. It will be implemented as a new, manually created service that other services will consume. Scaffolding can be used for managing user preferences.

*Execute from `Diquis.Application/Services`:*
```bash
# To manage user notification preferences
dotnet new nano-service -s UserNotificationPreference -p UserNotificationPreferences -ap Diquis
dotnet new nano-controller -s UserNotificationPreference -p UserNotificationPreferences -ap Diquis
```

### 3. Implementation Plan

#### User Story: "Smart" Alert System & Broadcasts
**As a** user, **I want** to receive timely, relevant notifications on my preferred channels.

**Technical Tasks:**
1.  **Domain:** Create the `Notification` and `UserNotificationPreference` entities.
2.  **Persistence:** Add `DbSet`s for both to `ApplicationDbContext` and create a migration.
    - *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddNotificationEntities`
3.  **Application (Interfaces):**
    -   Create `INotificationService` in the Application layer.
    -   Create `IPushNotificationService` in the Application layer.
4.  **Infrastructure (Implementation):**
    -   Manually create `NotificationService.cs`. This will be the central orchestrator.
    -   Implement `FirebasePushNotificationService.cs` (or similar for another provider) that implements `IPushNotificationService`.
5.  **Application (Service Logic):** `NotificationService` will expose methods like `SendNotificationAsync(NotificationRequest request)` and `BroadcastToGroupAsync(BroadcastRequest request)`.
    -   **Crucial Routing Logic:** `SendNotificationAsync` will:
        1.  Fetch the user's `UserNotificationPreference`.
        2.  Check the event's `Criticality`. If `Critical`, override preferences and send to all available channels (`Email`, `Push`).
        3.  If not `Critical`, check if the user has opted for a "Daily Digest" for this `EventType`. If so, create the `Notification` record in the DB and stop.
        4.  Otherwise, iterate the user's preferred channels and enqueue background jobs for each (`_jobService.Enqueue<IEmailService>(...)`, `_jobService.Enqueue<IPushNotificationService>(...)`).
6.  **Infrastructure (Background Job):**
    -   Create a recurring Hangfire job, `DailyDigestJob`, to run nightly. This job will query for all informational `Notification` records created in the last 24 hours, group them by user, and send a summary email.

---

## Module 14: 📥 Data Portability & Migration Tools

This module is process-oriented, focused on background jobs for importing and exporting tenant data.

### 1. Architectural Analysis

- **Domain Entities:** None. This module operates on existing entities.
- **Multi-Tenancy Scope:** All import/export operations are strictly tenant-scoped.
- **Permissions:** `academy_owner` and `super_user` have access. A new policy `CanManageDataPortability` will be created.

### 2. Scaffolding Strategy (CLI)
**Not applicable.** This module consists of manually created services, controllers, and background jobs.

### 3. Implementation Plan

#### User Story: The "Smart Import" Wizard
**As a** new owner, **I want to** upload a CSV of my players to quickly populate the system.

**Technical Tasks:**
1.  **API:** Create a new `DataPortabilityController.cs` in `Diquis.WebApi`.
    -   `POST /api/data-portability/import/players`: A `multipart/form-data` endpoint secured by the `CanManageDataPortability` policy.
2.  **Application (Service):** Create a new `IDataPortabilityService` and `DataPortabilityService.cs`.
    -   The controller action will call `_dataPortabilityService.InitiatePlayerImportAsync(IFormFile file)`.
    -   This service method will:
        1.  Validate file type and size.
        2.  Save the file to a temporary location.
        3.  Enqueue a background job: `_jobService.Enqueue<IDataPortabilityJob>(j => j.ProcessPlayerImport(file.Id, _currentUserService.TenantId))`.
        4.  Return the job ID to the client for status polling.
3.  **Infrastructure (Background Job):** Create `DataPortabilityJob.cs`.
    -   The `ProcessPlayerImport` method is a multi-step process.
    -   **Step 1 (Mapping & Validation):** Use a library like `CsvHelper` to parse the file. Implement the "best guess" column mapping logic. Perform a validation-only "dry run" on all rows. If errors are found, save the error report and update the job status to "FailedValidation".
    -   **Step 2 (Bulk Creation):** If validation passes, use a bulk-insert library (e.g., `EFCore.BulkExtensions`) to create the `ApplicationUser` records efficiently. Update job status to "Completed".

#### User Story: Export Your Data
**As an** owner, **I want to** download all my historical data for external analysis.

**Technical Tasks:**
1.  **API:** In `DataPortabilityController.cs`, add `POST /api/data-portability/export`.
2.  **Application (Service):**
    -   The API calls `_dataPortabilityService.InitiateExportAsync(ExportDataRequest request)`.
    -   This method enqueues a background job: `_jobService.Enqueue<IDataPortabilityJob>(j => j.ProcessExport(request, _currentUserService.TenantId))`.
3.  **Infrastructure (Background Job):** In `DataPortabilityJob.cs`:
    -   `ProcessExport` will:
        1.  Fetch the requested data sets (e.g., all `Match` records for the tenant).
        2.  Serialize each data set to a CSV/JSON string.
        3.  Create a `.zip` archive in memory.
        4.  Upload the zip file to a secure, temporary location (e.g., Azure Blob Storage with a short-lived SAS token).
        5.  Enqueue a notification job to email the secure download link to the user.

---

## Module 15: ✍️ Digital Signatures & Documents

### 1. Architectural Analysis

#### Domain Entities

1.  **`DocumentTemplate`**: An admin-managed template for a signable document.
    *   `Title` (string, required).
    *   `Content` (string, required, HTML/Markdown with placeholders like `{{PlayerName}}`).
    *   `DocumentType` (enum: `Waiver`, `Consent`, `Contract`).
2.  **`SignedDocument`**: An instance of a document that has been signed.
    *   `PlayerId` (Guid, required, FK to `ApplicationUser`).
    *   `SignatoryId` (Guid, required, FK to `ApplicationUser` - the parent or player who signed).
    *   `DocumentTemplateId` (Guid, required, FK to `DocumentTemplate`).
    *   `SealedPdfUrl` (string, required): A link to the final, tamper-proof PDF.
    *   `SignatureAuditTrail` (JSON, string): Contains IP, timestamp, user agent, etc.
    *   `ExpiresAt` (DateTime, nullable).

#### Multi-Tenancy Scope
- **`IMustHaveTenant` Required:** `DocumentTemplate`, `SignedDocument`.

### 2. Scaffolding Strategy (CLI)

*Execute from `Diquis.Application/Services`:*
```bash
# To manage the document templates
dotnet new nano-service -s DocumentTemplate -p DocumentTemplates -ap Diquis
dotnet new nano-controller -s DocumentTemplate -p DocumentTemplates -ap Diquis
```
The core signing logic will be in a manually created `IDigitalSignatureService`.

### 3. Implementation Plan

#### User Story: Waiver & Consent Management & Blocker Logic
**As a** director, **I need** to block players from participation until their parents sign required waivers.

**Technical Tasks:**
1.  **Domain:** Create `DocumentTemplate` and `SignedDocument`. Add a `WaiverStatus` enum (`Current`, `Pending`, `Expired`) to the `ApplicationUser` entity.
2.  **Persistence:** Add `DbSet`s, and create migrations for both `ApplicationDbContext` (for the new entities) and `BaseDbContext` (for the `WaiverStatus` on `ApplicationUser`).
3.  **Application (Service):**
    -   Create `IDigitalSignatureService` and `DigitalSignatureService.cs`.
    -   `GenerateAndSendSignatureRequestAsync(Guid playerId, Guid templateId)`:
        1.  Generates a PDF from the template using `QuestPDF`, replacing placeholders.
        2.  Sends an email with a secure link (containing a short-lived JWT) to the parent.
    -   `FinalizeSignatureAsync(FinalizeSignatureRequest request)`:
        1.  Validates the JWT from the link.
        2.  Captures signature data and audit trail.
        3.  "Seals" the PDF by adding the signature and audit trail, then stores it.
        4.  Creates the `SignedDocument` record.
        5.  Updates the `ApplicationUser.WaiverStatus` to `Current`.
4.  **Crucial "Blocker" Logic:**
    -   Modify `TeamService.UpdateRosterAsync()`: Throw a `ValidationException` if a coach tries to add a player whose `WaiverStatus` is not `Current`.
    -   Modify `TrainingSessionService.MarkAttendanceAsync()`: Prevent a player's attendance from being marked `Present` if their `WaiverStatus` is not `Current`.
5.  **Infrastructure (Background Job):**
    -   Create a recurring job `DocumentExpiryJob` that runs nightly, finds documents expiring soon, and calls `INotificationService` to alert users and admins.

## 4. Code Specifications (Key Logic)

### `NotificationService.cs` - Core Routing Logic

```csharp
// Inside NotificationService.cs
public async Task SendNotificationAsync(NotificationRequest request)
{
    // Simplified logic
    var preferences = await _context.UserNotificationPreferences.FindAsync(request.UserId);
    
    bool isCritical = request.Criticality == Criticality.Critical;
    bool wantsDigest = preferences?.Preferences.GetValueOrDefault(request.EventType)?.Digest ?? false;

    if (isCritical) 
    {
        // Force send to all channels
        _jobService.Enqueue<IPushNotificationService>(s => s.SendAsync(request.UserId, ...));
        _jobService.Enqueue<IEmailService>(s => s.SendAsync(request.UserId, ...));
    }
    else if (wantsDigest) 
    {
        // Save to DB for daily digest job to pick up
        _context.Notifications.Add(new Notification { ... });
        await _context.SaveChangesAsync();
    }
    else 
    {
        // Send via preferred channels from `preferences`
    }
}
```

### `DataPortabilityJob.cs` - Pre-Import Validation Logic

```csharp
// Inside DataPortabilityJob.cs
public async Task ProcessPlayerImport(Guid fileId, string tenantId)
{
    // Set tenant context
    // Download file from temp storage
    var errors = new List<string>();
    var records = new List<PlayerRecord>();

    using (var reader = new StreamReader(filePath))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        // Auto-map headers to DTO
        // csv.Context.RegisterClassMap<PlayerImportMap>();
        records = csv.GetRecords<PlayerRecord>().ToList();
    }

    // Dry Run Validation
    for(int i = 0; i < records.Count; i++)
    {
        var record = records[i];
        if (string.IsNullOrWhiteSpace(record.FullName))
            errors.Add($"Row {i+2}: FullName is missing.");
        if (!IsValidEmail(record.Email))
            errors.Add($"Row {i+2}: Invalid email format for '{record.Email}'.");
        // ... more validation ...
    }

    if (errors.Any())
    {
        // Update job status to FailedValidation and save `errors` report
        return; 
    }

    // If no errors, proceed with bulk insertion
    // await _bulkInserter.BulkInsertAsync(records);
    // Update job status to Completed
}
```

### `DigitalSignatureService.cs` - Blocker Logic Integration Point

```csharp
// Inside PlayerService.cs - (Modified from Player Management)

public override async Task<Guid> CreateAsync(CreatePlayerRequest request)
{
    // ... existing logic to create player ...
    
    // Set initial status to require waiver
    newUser.WaiverStatus = WaiverStatus.Pending;
    var result = await _userManager.UpdateAsync(newUser);
    
    // Enqueue a job to send the signature request
    _jobService.Enqueue<IDigitalSignatureService>(s => s.GenerateAndSendSignatureRequest(newUser.Id, standardWaiverTemplateId));

    return newUser.Id;
}
```
