namespace Diquis.Domain.Enums
{
    /// <summary>
    /// Represents the provisioning status of a tenant during creation or update operations.
    /// </summary>
    public enum ProvisioningStatus
    {
        /// <summary>
        /// The tenant has been created but provisioning has not started.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The tenant is currently being provisioned (database creation, admin user setup, etc.).
        /// </summary>
        Provisioning = 1,

        /// <summary>
        /// The tenant has been successfully provisioned and is active.
        /// </summary>
        Active = 2,

        /// <summary>
        /// The tenant provisioning has failed.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// The tenant is currently being updated.
        /// </summary>
        Updating = 4
    }
}
