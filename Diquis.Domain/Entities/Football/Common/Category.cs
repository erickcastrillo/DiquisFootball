using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Entities.Football.Common
{
    /// <summary>
    /// Represents a football category, such as an age group or competition level.
    /// </summary>
    /// <remarks>
    /// Each <see cref="Category"/> is associated with a <see cref="Locale"/> for localization
    /// and maintains a collection of <see cref="Player"/> entities assigned to this category.
    /// Inherits auditing and tenant information from <see cref="AuditableEntity"/>.
    /// </remarks>
    public class Category : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the category (e.g., "U12", "Senior", "Women").
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the locale for this category, indicating the language or region.
        /// </summary>
        /// <remarks>
        /// The <see cref="Locale"/> property is used for localization purposes, allowing the category
        /// to be associated with a specific language or regional setting.
        /// </remarks>
        public required Locale Locale { get; set; }

        /// <summary>
        /// Gets or sets the collection of players assigned to this category.
        /// </summary>
        /// <remarks>
        /// This is an inverse navigation property to <see cref="Player"/> entities that belong to this category.
        /// Initialized to an empty <see cref="HashSet{Player}"/> to prevent null reference issues.
        /// </remarks>
        public ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
