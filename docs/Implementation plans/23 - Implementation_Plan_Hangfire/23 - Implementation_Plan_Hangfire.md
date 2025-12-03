# Implementation Plan: Background Jobs (Hangfire) & Real-Time Notifications

**Status:** Draft
**Owner:** Technical Lead
**Reference:**
*   FRS: `docs/Business Requirements/BackgroundJobs_Hangfire_FRS.md`
*   Tech Guide: `docs/Technical documentation/BackgroundJobs_Hangfire_TechnicalGuide.md`

## Overview
This plan outlines the steps to integrate **Hangfire** for background processing and **SignalR** for real-time user feedback. The primary goal is to offload the resource-intensive "Tenant Provisioning" process, ensuring a responsive UI for both Admins and public users.

---

## Phase 1: Infrastructure Plumbing (Hangfire)
**Goal:** Enable the application to enqueue and execute background jobs reliably using PostgreSQL as the storage provider.

### 1.1. Dependencies & Configuration
*   **Action:** Install NuGet packages in `Diquis.Infrastructure` and `Diquis.WebApi`.
    *   `Hangfire.Core`
    *   `Hangfire.AspNetCore`
    *   `Hangfire.PostgreSql` (Note: Using PostgreSQL storage as per current architecture).
*   **Action:** Configure Hangfire in `Program.cs` / `Startup.cs`.
    *   Register Hangfire Server (`AddHangfireServer`).
    *   Configure Global Configuration to use PostgreSQL Storage.
    *   **Critical:** Ensure serializer settings (Newtonsoft/System.Text.Json) are compatible with existing domain objects.

### 1.2. Abstraction Layer
*   **Action:** Create `IBackgroundJobService` in `Diquis.Application`.
    *   Define methods: `Enqueue(Expression<Action> methodCall)` and `Enqueue(Expression<Func<Task>> methodCall)`.
*   **Action:** Implement `HangfireJobService` in `Diquis.Infrastructure`.
    *   Wrap `BackgroundJob.Enqueue`.
*   **Action:** Register `IBackgroundJobService` -> `HangfireJobService` in the DI container.

### 1.3. Dashboard & Security
*   **Action:** Implement `HangfireAuthorizationFilter` in `Diquis.WebApi`.
    *   Logic: Allow access only if `User.Identity.IsAuthenticated` AND `User.IsInRole("SuperAdmin")`.
*   **Action:** Map the dashboard endpoint: `app.UseHangfireDashboard("/hangfire", options)`.

---

## Phase 2: Real-Time Infrastructure (SignalR)
**Goal:** Establish a WebSocket channel to push updates to connected clients.

### 2.1. Backend Hub
*   **Action:** Create `NotificationHub` in `Diquis.Infrastructure` (or `WebApi` depending on layering preference, usually Infra or a dedicated SignalR project).
    *   Inherit from `Hub`.
    *   Define methods for client-to-server communication if needed (mostly server-to-client for this scope).
*   **Action:** Register SignalR in `Program.cs` (`services.AddSignalR()`) and map the endpoint (`app.MapHub<NotificationHub>("/hubs/notifications")`).

### 2.2. Authentication
*   **Action:** Configure JWT Bearer Auth to read the token from the Query String (`access_token`) for the Hub path.
    *   *Reason:* WebSockets cannot send custom headers.

### 2.3. Frontend Client (React)
*   **Action:** Install `@microsoft/signalr` package.
*   **Action:** Create `SignalRContext` and `SignalRProvider`.
*   **Action:** Implement `useSignalR` hook.
    *   Handle connection lifecycle (Start, Stop, Reconnect).
    *   Expose a method to subscribe to specific events (e.g., `on("ReceiveNotification")`).

---

## Phase 3: The Tenant Job (Refactoring)
**Goal:** Move the synchronous provisioning logic into a background job.

### 3.1. Domain Updates
*   **Action:** Add `ProvisioningStatus` Enum to `Diquis.Domain`.
    *   Values: `Pending`, `Provisioning`, `Active`, `Failed`.
*   **Action:** Update `Tenant` entity to include `ProvisioningStatus` property.
*   **Action:** Create EF Core migration to update the database schema.

### 3.2. Job Implementation
*   **Action:** Create `ProvisionTenantJob` class in `Diquis.Application`.
    *   Inject `ITenantRepository`, `IDatabaseMigrator`, `IHubContext<NotificationHub>`.
    *   **Logic:**
        1.  Fetch Tenant.
        2.  Update Status -> `Provisioning`.
        3.  Run Migrations/Seeding (The slow part).
        4.  Update Status -> `Active`.
        5.  Handle Exceptions -> Update Status `Failed`.

### 3.3. Service Refactoring
*   **Action:** Modify `CreateTenantCommandHandler` (or `TenantService`).
    *   **Old Flow:** Create DB -> Return Result.
    *   **New Flow:**
        1.  Create Tenant Record (Status = `Pending`).
        2.  `_backgroundJobService.Enqueue(() => _provisionTenantJob.ExecuteAsync(tenant.Id, currentUserId))`.
        3.  Return `202 Accepted` (or success with "Pending" status).

---

## Phase 4: Notification Strategy
**Goal:** route feedback to the correct channel based on the user context.

### 4.1. Notification Logic
*   **Action:** Update `ProvisionTenantJob` to handle the notification logic.
    *   **Input:** The Job should accept `Guid? initiatingUserId` and `string email`.
    *   **Condition:**
        *   If `initiatingUserId` is present (Admin Scenario): Call `_hubContext.Clients.User(initiatingUserId).SendAsync("ReceiveNotification", ...)`
        *   If `initiatingUserId` is null (Public Scenario): Call `_emailService.SendWelcomeEmail(email)`.

### 4.2. Frontend Toast Integration
*   **Action:** In the Admin Dashboard Layout, use the `useSignalR` hook.
*   **Action:** Listen for `ReceiveNotification`.
*   **Action:** Trigger a Global Toast/Snackbar when a message is received.

---

## 5. Testing Strategy

### 5.1. Unit Testing
*   **Jobs:** Unit test `ProvisionTenantJob` by mocking `ITenantRepository` and `IDatabaseMigrator`.
    *   Verify that `Status` is updated to `Active` on success.
    *   Verify that `Status` is updated to `Failed` on exception.
*   **Services:** Unit test the Command Handler to verify it calls `_backgroundJobService.Enqueue`.

### 5.2. Integration Testing
*   **Hangfire:** Use `Hangfire.MemoryStorage` for integration tests to avoid needing a real Postgres instance during test runs (optional, but recommended for speed).
*   **SignalR:** Difficult to automate fully. Rely on manual verification:
    1.  Open Admin Dashboard (Browser A).
    2.  Trigger Tenant Creation.
    3.  Verify Toast appears in Browser A.
    4.  Verify Database is created.
