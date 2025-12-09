namespace Diquis.Application.Common.Notifications
{
    /// <summary>
    /// Defines the contract for sending real-time notifications to users.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Notifies a user that a tenant has been successfully created and provisioned.
        /// </summary>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="tenantId">The ID of the created tenant.</param>
        /// <param name="tenantName">The name of the created tenant.</param>
        Task NotifyTenantCreatedAsync(string userId, string tenantId, string tenantName);

        /// <summary>
        /// Notifies a user that tenant creation has failed.
        /// </summary>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="error">The error message describing why the creation failed.</param>
        Task NotifyTenantCreationFailedAsync(string userId, string error);

        /// <summary>
        /// Notifies a user that a tenant has been successfully updated.
        /// </summary>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="tenantId">The ID of the updated tenant.</param>
        /// <param name="tenantName">The name of the updated tenant.</param>
        Task NotifyTenantUpdatedAsync(string userId, string tenantId, string tenantName);

        /// <summary>
        /// Notifies a user that tenant update has failed.
        /// </summary>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="tenantId">The ID of the tenant that failed to update.</param>
        /// <param name="error">The error message describing why the update failed.</param>
        Task NotifyTenantUpdateFailedAsync(string userId, string tenantId, string error);
    }
}
