# SignalR Real-Time Notification Setup

## Changes Made

### 1. Created SignalR Infrastructure

**File: `Diquis.Infrastructure/Services/NotificationHub.cs`**
- SignalR hub for real-time notifications

**File: `Diquis.Infrastructure/Services/SignalRNotificationService.cs`**
- Implementation of `INotificationService` using SignalR
- Sends real-time updates to connected clients

### 2. Updated Service Registration

**File: `Diquis.BackgroundJobs/Extensions/ServiceCollectionExtensions.cs`**
- Added `services.AddSignalR()` to enable SignalR
- Changed from `LoggingNotificationService` to `SignalRNotificationService`
- Added `SchedulePollingInterval = TimeSpan.FromSeconds(1)` for faster Hangfire job processing

## Required Additional Changes

### Add SignalR Hub Endpoint Mapping

In your `Diquis.BackgroundJobs/Program.cs`, add the following line **after** `app.UseRouting()` and **before** `app.Run()`:

```csharp
using Diquis.Infrastructure.Services;

// ... existing code ...

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Add this line to map the SignalR hub
app.MapHub<NotificationHub>("/hubs/notifications");

// ... rest of your code ...
app.Run();
```

### Frontend JavaScript Client

Add this JavaScript to your tenant management page to receive real-time updates:

```javascript
// Add SignalR client library reference in your _Layout.cshtml or page:
// <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notifications")
    .withAutomaticReconnect()
    .build();

// Listen for tenant status updates
connection.on("TenantStatusUpdate", function (data) {
    console.log("Tenant status update:", data);
    
    // Update the UI based on the status
    const tenantRow = document.querySelector(`[data-tenant-id="${data.tenantId}"]`);
    if (tenantRow) {
        const statusElement = tenantRow.querySelector('.tenant-status');
        if (statusElement) {
            statusElement.textContent = data.status;
            statusElement.className = `tenant-status status-${data.status.toLowerCase()}`;
        }
        
        if (data.message) {
            const messageElement = tenantRow.querySelector('.tenant-message');
            if (messageElement) {
                messageElement.textContent = data.message;
            }
        }
    }
});

// Start the connection
connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR connection error:", err));
```

### Update ProvisionTenantJob

Ensure your `ProvisionTenantJob` calls the notification service at key points:

```csharp
// At the start
await _notificationService.SendTenantStatusUpdateAsync(
    tenantId, 
    "Provisioning", 
    "Starting tenant provisioning..."
);

// On success
await _notificationService.SendTenantStatusUpdateAsync(
    tenantId, 
    "Active", 
    "Tenant provisioned successfully"
);

// On error
await _notificationService.SendTenantStatusUpdateAsync(
    tenantId, 
    "Failed", 
    $"Provisioning failed: {ex.Message}"
);
```

## Testing

1. Start both the main application and the background jobs application
2. Create a new tenant
3. Watch the browser console for SignalR connection and status updates
4. The tenant status should update in real-time without page refresh

## Troubleshooting

- **SignalR not connecting**: Check that `/hubs/notifications` endpoint is mapped in Program.cs
- **No status updates**: Verify `SignalRNotificationService` is registered instead of `LoggingNotificationService`
- **CORS issues**: If frontend and backend are on different ports, configure CORS for SignalR
