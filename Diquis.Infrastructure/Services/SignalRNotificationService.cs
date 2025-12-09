using Diquis.Application.Common.Notifications;
using Diquis.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Diquis.Infrastructure.Services;

/// <summary>
/// SignalR-based notification service with Redis backplane support.
/// Works across multiple application instances and processes.
/// </summary>
public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<SignalRNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyTenantCreatedAsync(string userId, string tenantId, string tenantName)
    {
        _logger.LogInformation("Sending tenant created notification for {TenantId}", tenantId);
        
        await _hubContext.Clients.All.SendAsync("TenantCreated", new
        {
            type = "success",
            message = $"Tenant '{tenantName}' has been created successfully!",
            userId,
            tenantId,
            tenantName,
            timestamp = DateTime.UtcNow.ToString("o")
        });
    }

    public async Task NotifyTenantCreationFailedAsync(string userId, string error)
    {
        _logger.LogWarning("Sending tenant creation failed notification: {Error}", error);
        
        await _hubContext.Clients.All.SendAsync("TenantCreationFailed", new
        {
            type = "error",
            message = $"Failed to create tenant: {error}",
            userId,
            error,
            timestamp = DateTime.UtcNow.ToString("o")
        });
    }

    public async Task NotifyTenantUpdatedAsync(string userId, string tenantId, string tenantName)
    {
        _logger.LogInformation("Sending tenant updated notification for {TenantId}", tenantId);
        
        await _hubContext.Clients.All.SendAsync("TenantUpdated", new
        {
            type = "success",
            message = $"Tenant '{tenantName}' has been updated successfully!",
            userId,
            tenantId,
            tenantName,
            timestamp = DateTime.UtcNow.ToString("o")
        });
    }

    public async Task NotifyTenantUpdateFailedAsync(string userId, string tenantId, string error)
    {
        _logger.LogWarning("Sending tenant update failed notification for {TenantId}: {Error}", tenantId, error);
        
        await _hubContext.Clients.All.SendAsync("TenantUpdateFailed", new
        {
            type = "error",
            message = $"Failed to update tenant: {error}",
            userId,
            tenantId,
            error,
            timestamp = DateTime.UtcNow.ToString("o")
        });
    }
}
