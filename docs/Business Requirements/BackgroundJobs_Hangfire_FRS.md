# Functional Requirement Specification: Background Processing Module (Hangfire)

**Version:** 1.0
**Date:** 2025-12-03
**Status:** Draft

## 1. Executive Summary
The "Nano ASP.NET Boilerplate" currently processes requests synchronously. For resource-intensive operations like **Tenant Provisioning** (which involves database creation and data seeding), this approach blocks the user interface and limits scalability.

This module introduces **Hangfire** to handle long-running tasks asynchronously. By offloading these tasks to a background worker, we ensure the UI remains responsive and the system can handle higher loads. Additionally, we will integrate **SignalR** to provide real-time feedback to users when tasks complete.

## 2. Key Actors
*   **Super Admin:** An internal administrator responsible for managing tenants and monitoring system health.
*   **Prospective Client:** A public user signing up for a new tenant account.

## 3. Functional Capabilities

### 3.1. Job Queueing & Execution
*   **Reliable Execution:** The system must use Hangfire to queue and execute background jobs.
*   **Persistence:** Jobs must be persisted to the database (SQL Server) to survive application restarts.
*   **Retries:** Failed jobs must automatically retry a configurable number of times before being moved to the Dead Letter Queue.
*   **Job Types:** Support for "Fire-and-Forget" jobs is required for the initial implementation.

### 3.2. Real-Time Feedback (SignalR)
*   **Notifications:** The system must be able to push real-time notifications to connected clients.
*   **Targeting:** Notifications must be targeted to specific users (e.g., the Admin who initiated the action).
*   **UI Integration:** The frontend must display these notifications as "Toast" messages.

### 3.3. Monitoring & Management
*   **Hangfire Dashboard:** A secured UI (accessible only to Super Admins) to view enqueued, processing, succeeded, and failed jobs.
*   **Manual Intervention:** Admins must be able to manually trigger or delete jobs via the dashboard.

## 4. Detailed Workflows

### 4.1. Scenario A: Async Tenant Provisioning (Admin Dashboard)
**Context:** A Super Admin manually creates a new tenant from the internal dashboard.

1.  **Submission:** Admin fills out the "Create Tenant" form and clicks "Submit".
2.  **Queueing:** The API validates the request, creates a "Tenant" record with status `Provisioning`, enqueues a `ProvisionTenantJob` in Hangfire, and immediately returns a `202 Accepted` response.
3.  **UI Response:** The Admin Dashboard displays a message: *"Tenant creation started. You will be notified when complete."* The UI is unblocked.
4.  **Processing:** The Background Worker picks up the job:
    *   Creates the Tenant Database.
    *   Runs Migrations.
    *   Seeds initial data.
5.  **Completion:**
    *   The job updates the Tenant status to `Active`.
    *   The job invokes the `NotificationHub` to send a "Success" message to the Admin's User ID.
6.  **Notification:** The Admin's browser receives the SignalR message and displays a green Toast notification: *"Tenant [Name] has been successfully provisioned."*

### 4.2. Scenario B: Public Sign-up
**Context:** A user registers for a new account on the public landing page.

1.  **Submission:** User fills out the "Sign Up" form and clicks "Register".
2.  **Queueing:** The API validates the request, creates a "Tenant" record with status `Provisioning`, enqueues a `ProvisionTenantJob` in Hangfire, and returns a `202 Accepted` response.
3.  **UI Response:** The Public UI displays a success page: *"Thanks for signing up! Check your email shortly for your login details."*
4.  **Processing:** The Background Worker picks up the job:
    *   Creates the Tenant Database.
    *   Runs Migrations.
    *   Seeds initial data.
5.  **Completion:**
    *   The job updates the Tenant status to `Active`.
    *   The job triggers an Email Service to send a "Welcome" email with login credentials to the user.
    *   *Note: No SignalR notification is sent to the public user's browser as they may have closed the tab.*

## 5. Acceptance Criteria

*   **AC1:** Long-running tasks (specifically Tenant Provisioning) do not block the HTTP request thread for more than 500ms.
*   **AC2:** If the `ProvisionTenantJob` fails, the Tenant status is updated to `Failed`, and the error is logged in the Hangfire Dashboard.
*   **AC3:** In Scenario A, the Admin receives a visible Toast notification within 5 seconds of job completion (assuming active connection).
*   **AC4:** In Scenario B, the User receives a Welcome Email upon successful provisioning.
*   **AC5:** The Hangfire Dashboard is accessible at `/hangfire` but restricted to authenticated Super Admins only.
*   **AC6:** If the application server restarts while a job is running, the job is re-queued and processed again automatically.
