using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Enums;

namespace Diquis.Domain.Entities.Multitenancy
{
    /// <summary>
    /// Represents a tenant in a multitenant application.
    /// Contains information such as tenant ID, name, connection string, status, and creation date.
    /// Inherits from <see cref="BaseEntity{TKey}"/> with string as the key type.
    /// </summary>
    public class Tenant : BaseEntity<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the tenant.
        /// This property is not auto-generated; magic-strings are used for simplicity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new string Id { get; set; } // for simplicity, using magic-strings for tenant ids

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the tenant's database.
        /// If null, the tenant uses the main shared database.
        /// </summary>
        public string ConnectionString { get; set; } // any tenant with a null value for ConnectionString will use the main shared database

        /// <summary>
        /// Gets or sets a value indicating whether the tenant is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the tenant was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the current provisioning status of the tenant.
        /// </summary>
        public ProvisioningStatus Status { get; set; } = ProvisioningStatus.Pending;

        /// <summary>
        /// Gets or sets the error message if provisioning failed.
        /// </summary>
        public string? ProvisioningError { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last provisioning attempt.
        /// </summary>
        public DateTime? LastProvisioningAttempt { get; set; }
    }
}
