using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Entities.Football.Common
{
    /// <summary>
    /// Represents a football division, grouping players and associated with a specific <see cref="Locale"/>.
    /// </summary>
    /// <remarks>
    /// Inherits auditing and tenant information from <see cref="AuditableEntity"/>.
    /// </remarks>
    public class Division : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the division.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Locale"/> associated with this division.
        /// </summary>
        public Locale Locale { get; set; }

        /// <summary>
        /// Gets or sets the minimum age required for eligibility.
        /// </summary>
        public int MinAge { get; set; }

        /// <summary>
        /// Gets or sets the maximum age, in seconds, that a cached item is considered valid.
        /// </summary>
        public int MaxAge { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="Player"/> entities assigned to this division.
        /// </summary>
        /// <remarks>
        /// This is an optional inverse navigation property if divisions are assigned to players or teams.
        /// Initialized to an empty <see cref="HashSet{T}"/> to prevent null reference issues.
        /// </remarks>
        public ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
