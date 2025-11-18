namespace Diquis.Domain.Entities.Common
{
    /// <summary>
    /// Serves as the base class for domain entities that expose an identifier.
    /// </summary>
    /// <typeparam name="TId">The type used for the entity identifier (for example, <c>int</c>, <c>Guid</c>, or <c>string</c>).</typeparam>
    /// <remarks>
    /// Derive domain entity classes from <see cref="BaseEntity{TId}"/> to provide a common identifier property.
    /// The generic identifier type allows flexibility for different persistence strategies and key types.
    /// </remarks>
    public abstract class BaseEntity<TId>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        /// <value>The entity identifier of type <typeparamref name="TId"/>.</value>
        public TId Id { get; set; }
    }
}
