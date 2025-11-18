namespace Diquis.Domain.Entities.Common
{
    /// <summary>
    /// Defines audit information that an entity can implement to record who created
    /// the entity and when, as well as who last modified it and when.
    /// </summary>
    /// <remarks>
    /// Implementers should populate <see cref="CreatedBy"/> and <see cref="CreatedOn"/> when the entity
    /// is created. <see cref="LastModifiedBy"/> and <see cref="LastModifiedOn"/> are optional and
    /// may remain <c>null</c> until the entity is modified.
    /// </remarks>
    public interface IAuditableEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the user or system that created the entity.
        /// </summary>
        /// <value>A <see cref="System.Guid"/> representing the creator's identifier.</value>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time (UTC) when the entity was created.
        /// </summary>
        /// <value>A <see cref="System.DateTime"/> representing the creation timestamp (UTC).</value>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user or system that last modified the entity.
        /// </summary>
        /// <value>
        /// A nullable <see cref="System.Guid"/> representing the last modifier's identifier,
        /// or <c>null</c> if the entity has never been modified after creation.
        /// </value>
        public Guid? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time (UTC) when the entity was last modified.
        /// </summary>
        /// <value>
        /// A nullable <see cref="System.DateTime"/> representing the last modification timestamp (UTC),
        /// or <c>null</c> if the entity has not been modified since creation.
        /// </value>
        public DateTime? LastModifiedOn { get; set; }
    }
}
