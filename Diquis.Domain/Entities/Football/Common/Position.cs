using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Entities.Football.Common
{
    /// <summary>
    /// Represents a football playing position (e.g., Forward, Midfielder, Defender, Goalkeeper).
    /// </summary>
    /// <remarks>
    /// Each <see cref="Position"/> is associated with a <see cref="Locale"/> for localization
    /// and maintains a collection of <see cref="Player"/> entities who have this as their primary position.
    /// Inherits auditing and tenant information from <see cref="AuditableEntity"/>.
    /// </remarks>
    public class Position : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the position (e.g., "Forward", "Defender").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the locale for this position, indicating the language or region.
        /// </summary>
        public Locale Locale { get; set; }

        /// <summary>
        /// Gets or sets the collection of players who have this as their primary position.
        /// </summary>
        /// <remarks>
        /// This is an inverse navigation property to <see cref="Player.Position"/>.
        /// Initialized to an empty <see cref="HashSet{Player}"/> to prevent null reference issues.
        /// </remarks>
        public ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
