# Task 06: Logic - Refactor CreateTenantCommand

**Status:** Open
**Priority:** High
**Context:** Finally, we wire everything together. The "Create Tenant" command will no longer wait for the DB to be built. It will just queue the job and return.

## 1. Refactor Command Handler
*   **File:** `Diquis.Application/Tenants/Commands/CreateTenant/CreateTenantCommandHandler.cs`
*   **Action:** Inject `IBackgroundJobService`.
*   **Logic Change:**
    *   **Old:** Create Tenant Entity -> Create DB (Wait) -> Return ID.
    *   **New:**
        1.  Create Tenant Entity (Status = `Pending`). Save to Main DB.
        2.  `_jobService.Enqueue(() => _provisionJob.ExecuteAsync(tenant.Id, currentUserId, email));`
        3.  Return the Tenant ID (and potentially a 202 Accepted status if this is a controller action, or just the ID if it's CQRS).

## 2. Notification Logic (In the Job)
*   **File:** `Diquis.Application/Jobs/ProvisionTenantJob.cs`
*   **Action:** Finalize the notification logic inside the job (if not done in Task 04).
    *   `if (!string.IsNullOrEmpty(adminUserId))` -> Send SignalR message to that User ID.
    *   `else` -> Send Welcome Email to `userEmail`.

## 3. Validation
*   **Scenario A (Admin):** Create a tenant via Dashboard. UI should return immediately. Wait ~10s. See Toast Notification. Verify DB exists.
*   **Scenario B (Public):** Register via Sign-up form. UI shows "Check Email". Wait ~10s. Verify Email sent (mocked/logged). Verify DB exists.
