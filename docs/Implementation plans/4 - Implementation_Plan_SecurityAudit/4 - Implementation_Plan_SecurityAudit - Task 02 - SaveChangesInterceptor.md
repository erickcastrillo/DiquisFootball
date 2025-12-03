# Task Context
Implement the `AuditSaveChangesInterceptor` to automatically generate audit logs when changes are saved to the database. This interceptor will inspect the `ChangeTracker`, identify modified entities marked with `[Auditable]`, and create corresponding `AuditLog` entries in the same transaction.

# Core References
- **Plan:** [4 - Implementation_Plan_SecurityAudit.md](./4%20-%20Implementation_Plan_SecurityAudit.md)

# Step-by-Step Instructions
1.  **Create Interceptor Class:**
    *   Path: `Diquis.Infrastructure/Persistence/Interceptors/AuditSaveChangesInterceptor.cs`
    *   Inherit from `SaveChangesInterceptor`.
2.  **Inject Services:**
    *   Constructor should accept `ICurrentTenantUserService` to get the current user ID and tenant.
3.  **Implement `SavingChanges` / `SavingChangesAsync`:**
    *   Override both methods.
    *   Call a private helper `GenerateAuditEntries(eventData.Context)`.
    *   Call `base.SavingChanges...`.
4.  **Implement `GenerateAuditEntries`:**
    *   Detect changes (`context.ChangeTracker.DetectChanges()`).
    *   Iterate through entries. Skip if not `[Auditable]` or if state is Unchanged/Detached.
    *   Create `AuditLog` instance:
        *   `ActionType`: Entry state (Added -> CREATE, Modified -> UPDATE, Deleted -> DELETE).
        *   `OldValue`: Serialize `OriginalValues` (for Update/Delete).
        *   `NewValue`: Serialize `CurrentValues` (for Create/Update).
        *   `RecordId`: Serialize Primary Key.
    *   Add logs to `context.Set<AuditLog>()`.
5.  **Register Interceptor:**
    *   In `Diquis.Infrastructure/DependencyInjection.cs` (or Program.cs), register `AuditSaveChangesInterceptor` as Singleton or Scoped.
    *   Update `AddDbContext` configuration to use `.AddInterceptors(...)`.

# Acceptance Criteria
- [ ] `AuditSaveChangesInterceptor.cs` exists and implements logic.
- [ ] Interceptor is registered in DI and added to DbContext options.
- [ ] Integration test (if created) verifies logs are created on save.
