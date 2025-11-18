namespace Diquis.Domain.Entities.Common
{
    /// <summary>
    /// Defines members for entities that support soft deletion, allowing them to be marked as deleted without being
    /// physically removed from storage.
    /// </summary>
    /// <remarks>Implement this interface to enable soft delete functionality in entities, typically for
    /// scenarios where deleted records should be retained for auditing or recovery purposes. Soft-deleted entities can
    /// be excluded from queries or restored as needed.</remarks>
    public interface ISoftDelete
    {
        /// <summary>
        /// Gets or sets the date and time when the entity was deleted, if applicable.
        /// </summary>
        DateTime? DeletedOn { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier of the user who deleted the entity, if applicable.
        /// </summary>
        Guid? DeletedBy { get; set; }
    }
}
