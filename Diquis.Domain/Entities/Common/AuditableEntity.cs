using Diquis.Domain.Entities.Multitenancy;
using Diquis.Domain.Enums;

namespace Diquis.Domain.Entities.Common
{
    /// <summary>
    /// Base abstract entity that captures auditing information for domain entities and associates them with a tenant.
    /// </summary>
    /// <remarks>
    /// Inherit from this class for entities that require creation and modification audit fields as well as tenancy.
    /// Implementations or application services should populate <see cref="CreatedBy"/> and <see cref="CreatedOn"/> when
    /// an entity is created, and update <see cref="LastModifiedBy"/> and <see cref="LastModifiedOn"/> when the entity
    /// is modified. Date/time values should be stored in UTC.
    /// </remarks>
    /// <seealso cref="TenantBaseEntity"/>
    /// <seealso cref="IAuditableEntity"/>
    /// <seealso cref="IMustHaveTenant"/>
    public abstract class AuditableEntity : TenantBaseEntity, IAuditableEntity, IMustHaveTenant, ILocalizable
    {
        /// <summary>
        /// Identifier of the user who created the entity.
        /// </summary>
        /// <value>
        /// A <see cref="Guid"/> representing the creator's user identifier.
        /// </value>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was created.
        /// </summary>
        /// <value>
        /// A <see cref="DateTime"/> representing the creation timestamp. Prefer storing this value in UTC.
        /// </value>
        /// <remarks>
        /// This value should be set once on creation and not modified afterwards.
        /// </remarks>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Identifier of the user who last modified the entity, if any.
        /// </summary>
        /// <value>
        /// A nullable <see cref="Guid"/> representing the last modifier's user identifier, or <c>null</c> if the entity
        /// has never been modified since creation.
        /// </value>
        public Guid? LastModifiedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was last modified, if any.
        /// </summary>
        /// <value>
        /// A nullable <see cref="DateTime"/> representing the last modification timestamp. Prefer storing this value in UTC.
        /// </value>
        /// <remarks>
        /// This value should be updated whenever a persistent change is made to the entity.
        /// </remarks>
        public DateTime? LastModifiedOn { get; set; }
        
        /// <summary>
        /// The locale of the entity.
        /// </summary>
        public Locale Locale { get; set; }
    }
}
