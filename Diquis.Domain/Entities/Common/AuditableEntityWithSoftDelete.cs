using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities.Common
{
    /// <summary>
    /// Base entity that provides common auditing information and soft-delete metadata.
    /// </summary>
    /// <remarks>
    /// This abstract class combines tenant scoping (from <c>TenantBaseEntity</c>) with
    /// audit fields and soft-delete metadata so derived entities can record who and when
    /// they were created, modified, or marked as deleted.
    ///
    /// Be aware that soft-deleted entities will not automatically cascade soft deletes
    /// to related entities. Any cascading or cleanup behavior must be handled explicitly
    /// by the application.
    /// </remarks>
    public abstract class AuditableEntityWithSoftDelete : TenantBaseEntity, IAuditableEntity, ISoftDelete, IMustHaveTenant
    {
        /// <summary>
        /// Identifier of the principal that created the entity.
        /// </summary>
        /// <remarks>
        /// Typically a user id (GUID). This property is required for audit purposes.
        /// </remarks>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was created.
        /// </summary>
        /// <remarks>
        /// It is recommended to store values in UTC to avoid time zone ambiguity.
        /// </remarks>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Identifier of the principal that last modified the entity.
        /// </summary>
        /// <remarks>
        /// Nullable. When <c>null</c>, the entity has not been modified since creation.
        /// </remarks>
        public Guid? LastModifiedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was last modified.
        /// </summary>
        /// <remarks>
        /// Nullable. When <c>null</c>, the entity has not been modified since creation.
        /// It is recommended to store values in UTC.
        /// </remarks>
        public DateTime? LastModifiedOn { get; set; }

        /// <summary>
        /// Identifier of the principal that marked the entity as deleted (soft delete).
        /// </summary>
        /// <remarks>
        /// Nullable. When set, indicates who performed the soft-delete action.
        /// </remarks>
        public Guid? DeletedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was marked as deleted (soft delete).
        /// </summary>
        /// <remarks>
        /// Nullable. When set, indicates when the entity was soft-deleted.
        /// It is recommended to store values in UTC.
        /// </remarks>
        public DateTime? DeletedOn { get; set; }
    }
}
