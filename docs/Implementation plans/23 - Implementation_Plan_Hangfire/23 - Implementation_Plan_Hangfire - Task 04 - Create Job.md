# Task 04: Application - Create TenantProvisioningJob

**Status:** Open
**Priority:** High
**Context:** We are moving the heavy lifting of tenant creation (DB creation, migration, seeding) into a dedicated background job. This ensures the API responds immediately.

## 1. Domain Updates
*   **File:** `Diquis.Domain/Enums/ProvisioningStatus.cs`
    *   Create Enum: `Pending`, `Provisioning`, `Active`, `Failed`.
*   **File:** `Diquis.Domain/Entities/Tenant.cs`
    *   Add property: `public ProvisioningStatus Status { get; set; }`
*   **Action:** Create and run EF Core Migration to update the database.

## 2. Job Logic
*   **File:** `Diquis.Application/Jobs/ProvisionTenantJob.cs`
*   **Dependencies:** `ITenantRepository`, `IDatabaseMigrator` (or equivalent service that currently does the work), `IHubContext<NotificationHub>`.
*   **Action:** Implement `ExecuteAsync(Guid tenantId, string? adminUserId, string? userEmail)`.
    *   **Step 1:** Load Tenant. If null, return.
    *   **Step 2:** Update Status to `Provisioning`.
    *   **Step 3:** Call the heavy logic (Create DB, Migrate, Seed).
    *   **Step 4:** On Success: Update Status to `Active`.
    *   **Step 5:** On Failure: Catch Exception, Log it, Update Status to `Failed`.

## 3. Validation
*   Unit Test `ProvisionTenantJob`.
*   Mock the dependencies.
*   Verify that `Status` transitions correctly based on success/failure of the migrator.
