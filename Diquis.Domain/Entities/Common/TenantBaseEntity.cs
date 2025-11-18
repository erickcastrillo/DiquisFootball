using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities.Common
{
    /// <summary>
    /// Represents a base entity for multitenant scenarios, including a tenant identifier.
    /// Inherits from <see cref="BaseEntity{Guid}"/> and implements <see cref="IMustHaveTenant"/>.
    /// </summary>
    public abstract class TenantBaseEntity : BaseEntity<Guid>, IMustHaveTenant
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tenant to which this entity belongs.
        /// </summary>
        public string TenantId { get; set; }
    }
}
