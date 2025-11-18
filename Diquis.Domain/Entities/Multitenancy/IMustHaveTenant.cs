namespace Diquis.Domain.Entities.Multitenancy
{
    /// <summary>
    /// Interface to enforce that an entity must have a TenantId property.
    /// Used for multitenancy support to associate entities with a specific tenant.
    /// </summary>
    public interface IMustHaveTenant
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tenant that owns the entity.
        /// </summary>
        string TenantId { get; set; }
    }
}