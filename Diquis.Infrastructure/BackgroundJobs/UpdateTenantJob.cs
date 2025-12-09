using Diquis.Application.Common.Notifications;
using Diquis.Domain.Enums;
using Diquis.Infrastructure.Multitenancy.DTOs;
using Diquis.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Diquis.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Background job for updating tenant information.
    /// </summary>
    public class UpdateTenantJob
    {
        private readonly BaseDbContext _baseDbContext;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UpdateTenantJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateTenantJob"/> class.
        /// </summary>
        public UpdateTenantJob(
            BaseDbContext baseDbContext,
            INotificationService notificationService,
            ILogger<UpdateTenantJob> logger)
        {
            _baseDbContext = baseDbContext;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Executes the tenant update process.
        /// </summary>
        /// <param name="tenantId">The ID of the tenant to update.</param>
        /// <param name="request">The update request details.</param>
        /// <param name="initiatingUserId">The ID of the user who initiated the update.</param>
        public async Task ExecuteAsync(string tenantId, UpdateTenantRequest request, string initiatingUserId)
        {
            var tenant = await _baseDbContext.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                _logger.LogError("Tenant {TenantId} not found for update", tenantId);
                await _notificationService.NotifyTenantUpdateFailedAsync(initiatingUserId, tenantId, "Tenant not found");
                return;
            }

            if (tenant.Id == "root")
            {
                _logger.LogWarning("Attempt to edit root tenant by user {UserId}", initiatingUserId);
                await _notificationService.NotifyTenantUpdateFailedAsync(initiatingUserId, tenantId, "Cannot edit root tenant");
                return;
            }

            try
            {
                tenant.Status = ProvisioningStatus.Updating;
                await _baseDbContext.SaveChangesAsync();

                _logger.LogInformation("Updating tenant {TenantId}", tenantId);

                tenant.IsActive = request.IsActive;
                tenant.Name = request.Name;
                tenant.Status = ProvisioningStatus.Active;
                tenant.ProvisioningError = null;

                await _baseDbContext.SaveChangesAsync();

                _logger.LogInformation("Tenant {TenantId} updated successfully", tenantId);

                await _notificationService.NotifyTenantUpdatedAsync(initiatingUserId, tenant.Id, tenant.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update failed for tenant {TenantId}", tenantId);

                tenant.Status = ProvisioningStatus.Failed;
                tenant.ProvisioningError = ex.Message;
                await _baseDbContext.SaveChangesAsync();

                await _notificationService.NotifyTenantUpdateFailedAsync(initiatingUserId, tenantId, ex.Message);

                throw; // Re-throw to let Hangfire handle retries
            }
        }
    }
}
