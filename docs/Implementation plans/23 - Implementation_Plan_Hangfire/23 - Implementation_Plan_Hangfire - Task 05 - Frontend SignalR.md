# Task 05: Frontend - Create useSignalR Hook & Toast Handler

**Status:** Open
**Priority:** Medium
**Context:** The frontend needs to listen for real-time updates. We will create a reusable hook to manage the SignalR connection and a component to display notifications.

## 1. Dependencies
*   **Action:** `npm install @microsoft/signalr`

## 2. Custom Hook
*   **File:** `src/hooks/useSignalR.ts`
*   **Action:** Implement `useSignalR(hubUrl, token)`.
    *   Initialize `HubConnectionBuilder`.
    *   Handle `start()`, `stop()`, and automatic reconnects.
    *   Return the `connection` object.

## 3. Global Listener (Layout)
*   **File:** `src/layouts/AdminLayout.tsx` (or Main Layout)
*   **Action:** Use the hook to connect to `/hubs/notifications`.
*   **Action:** Subscribe to the `ReceiveNotification` event.
    ```typescript
    connection.on("ReceiveNotification", (type, message) => {
        // Use your Toast library here (e.g., react-hot-toast, notistack)
        toast[type.toLowerCase()](message);
    });
    ```

## 4. Validation
*   Hardcode a button in the UI that calls a test endpoint on the API.
*   Have the test endpoint call `_hubContext.Clients.All.SendAsync("ReceiveNotification", "Success", "Hello World")`.
*   Verify the Toast appears in the browser.
