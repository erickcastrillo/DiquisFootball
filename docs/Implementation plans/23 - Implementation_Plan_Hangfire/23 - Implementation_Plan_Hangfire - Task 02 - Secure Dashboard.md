# Task 02: Security - Secure Hangfire Dashboard & Configure Storage

**Status:** Open
**Priority:** High
**Context:** We need to configure the Hangfire Server and Dashboard. The Dashboard provides powerful control over jobs, so it MUST be secured to allow only Super Admins. We also need to configure PostgreSQL as the persistent storage.

## 1. Dashboard Authorization
*   **File:** `Diquis.WebApi/Filters/HangfireAuthorizationFilter.cs` (Create if not exists)
*   **Action:** Implement `IDashboardAuthorizationFilter`.
    *   Check `context.GetHttpContext().User.Identity.IsAuthenticated`.
    *   Check `context.GetHttpContext().User.IsInRole("SuperAdmin")`.

## 2. Configuration (Program.cs)
*   **File:** `Diquis.WebApi/Program.cs`
*   **Action:** Configure Hangfire services.
    *   Use `AddHangfire` with `UsePostgreSqlStorage`.
    *   **Important:** Configure JSON serialization settings to be compatible with your domain objects (handle circular refs if necessary, though DTOs are preferred).
    *   Call `AddHangfireServer()` to start the background processing server.
*   **Action:** Map the Dashboard endpoint.
    ```csharp
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new [] { new HangfireAuthorizationFilter() }
    });
    ```

## 3. Validation
*   Run the application.
*   Navigate to `/hangfire` as an anonymous user -> Should receive 401/403 or be redirected.
*   Navigate to `/hangfire` as a "Super Admin" -> Should see the Dashboard.
*   Check the PostgreSQL database -> Should see Hangfire tables created automatically.
