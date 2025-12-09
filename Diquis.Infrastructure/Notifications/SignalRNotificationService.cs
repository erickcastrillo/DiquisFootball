using Diquis.Application.Common.Notifications;
using Diquis.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Diquis.Infrastructure.Notifications
{
    /// <summary>
    /// Implementation of <see cref="INotificationService"/> using SignalR for real-time notifications.
    /// </summary>
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<SignalRNotificationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRNotificationService"/> class.
        /// </summary>
        /// <param name="hubContext">The SignalR hub context.</param>
        /// <param name="logger">The logger instance.</param>
        public SignalRNotificationService(
            IHubContext<NotificationHub> hubContext,
            ILogger<SignalRNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task NotifyTenantCreatedAsync(string userId, string tenantId, string tenantName)
        {
            _logger.LogInformation("Notifying user {UserId} about successful tenant creation: {TenantId}", userId, tenantId);
            
            await _hubContext.Clients.User(userId).SendAsync("TenantCreated", new
            {
                type = "success",
                message = $"Tenant '{tenantName}' has been successfully created and provisioned.",
                tenantId,
                tenantName,
                timestamp = DateTime.UtcNow
            });
        }

        /// <inheritdoc/>
        public async Task NotifyTenantCreationFailedAsync(string userId, string error)
        {
            _logger.LogWarning("Notifying user {UserId} about failed tenant creation: {Error}", userId, error);
            
            await _hubContext.Clients.User(userId).SendAsync("TenantCreationFailed", new
            {
                type = "error",
                message = $"Tenant creation failed: {error}",
                timestamp = DateTime.UtcNow
            });
        }

        /// <inheritdoc/>
        public async Task NotifyTenantUpdatedAsync(string userId, string tenantId, string tenantName)
        {
            _logger.LogInformation("Notifying user {UserId} about successful tenant update: {TenantId}", userId, tenantId);
            
            await _hubContext.Clients.User(userId).SendAsync("TenantUpdated", new
            {
                type = "success",
                message = $"Tenant '{tenantName}' has been successfully updated.",
                tenantId,
                tenantName,
                timestamp = DateTime.UtcNow
            });
        }

        /// <inheritdoc/>
        public async Task NotifyTenantUpdateFailedAsync(string userId, string tenantId, string error)
        {
            _logger.LogWarning("Notifying user {UserId} about failed tenant update: {TenantId} - {Error}", userId, tenantId, error);
            
            await _hubContext.Clients.User(userId).SendAsync("TenantUpdateFailed", new
            {
                type = "error",
                message = $"Tenant update failed: {error}",
                tenantId,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
