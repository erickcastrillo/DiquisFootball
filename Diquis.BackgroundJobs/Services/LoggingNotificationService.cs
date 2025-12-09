using Diquis.Application.Common.Notifications;
using Microsoft.Extensions.Logging;

namespace Diquis.BackgroundJobs.Services;

/// <summary>
/// Logging-only implementation of <see cref="INotificationService"/> for background jobs.
/// Since background jobs don't have access to SignalR hub context, notifications are logged instead.
/// </summary>
public class LoggingNotificationService : INotificationService
{
    private readonly ILogger<LoggingNotificationService> _logger;

    public LoggingNotificationService(ILogger<LoggingNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyTenantCreatedAsync(string userId, string tenantId, string tenantName)
    {
        _logger.LogInformation(
            "Tenant created notification: UserId={UserId}, TenantId={TenantId}, TenantName={TenantName}",
            userId, tenantId, tenantName);
        return Task.CompletedTask;
    }

    public Task NotifyTenantCreationFailedAsync(string userId, string error)
    {
        _logger.LogWarning(
            "Tenant creation failed notification: UserId={UserId}, Error={Error}",
            userId, error);
        return Task.CompletedTask;
    }

    public Task NotifyTenantUpdatedAsync(string userId, string tenantId, string tenantName)
    {
        _logger.LogInformation(
            "Tenant updated notification: UserId={UserId}, TenantId={TenantId}, TenantName={TenantName}",
            userId, tenantId, tenantName);
        return Task.CompletedTask;
    }

    public Task NotifyTenantUpdateFailedAsync(string userId, string tenantId, string error)
    {
        _logger.LogWarning(
            "Tenant update failed notification: UserId={UserId}, TenantId={TenantId}, Error={Error}",
            userId, tenantId, error);
        return Task.CompletedTask;
    }
}
