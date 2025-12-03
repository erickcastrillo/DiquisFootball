# Task 03: Infrastructure - Setup SignalR Hub

**Status:** Open
**Priority:** Medium
**Context:** To provide real-time feedback (e.g., "Tenant Provisioned"), we need a WebSocket channel. This task sets up the SignalR Hub and the necessary authentication handling.

## 1. Create Hub
*   **File:** `Diquis.Infrastructure/Hubs/NotificationHub.cs`
*   **Action:** Create a class inheriting from `Hub`.
    *   It can be empty for now, or contain a helper method `SendNotification(string userId, string message)`.
    *   Ideally, use `IHubContext<NotificationHub>` in services to push messages, rather than client-to-server calls.

## 2. Registration
*   **File:** `Diquis.WebApi/Program.cs`
*   **Action:** Add `builder.Services.AddSignalR()`.
*   **Action:** Map the hub: `app.MapHub<NotificationHub>("/hubs/notifications")`.

## 3. Authentication (JWT in Query String)
*   **File:** `Diquis.WebApi/Program.cs` (JWT Configuration section)
*   **Action:** Update the `AddJwtBearer` events.
    *   WebSockets cannot send Authorization headers. The token is sent in the query string `?access_token=...`.
    *   Implement `OnMessageReceived`:
        ```csharp
        if (path.StartsWithSegments("/hubs/notifications") && !string.IsNullOrEmpty(accessToken))
        {
            context.Token = accessToken;
        }
        ```

## 4. Validation
*   Run the application.
*   Use a tool like Postman (WebSocket mode) or a simple JS script to connect to `wss://localhost:port/hubs/notifications?access_token=VALID_TOKEN`.
*   Verify the connection is established (HTTP 101 Switching Protocols).
