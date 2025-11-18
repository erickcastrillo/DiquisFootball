using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Entities.Football.Common
{
    // Pseudocode plan:
    // 1. Add XML documentation to the Revision class describing its purpose as a snapshot
    //    of a player's skill ratings with metadata from AuditableEntity.
    // 2. Add XML summary/remarks for the PlayerId property explaining it's a foreign key.
    // 3. Add XML summary/remarks for the Player navigation property explaining the relation.
    // 4. Add XML summary for Comment explaining its use as an optional reviewer note.
    // 5. Add XML summary/remarks for ReviewerId and Reviewer similar to Player properties.
    // 6. Add XML summary/remarks for PlayerSkills collection explaining it contains the
    //    PlayerSkill entries that were recorded as part of this revision and note the
    //    default initialization to a HashSet to avoid null references.
    // 7. Ensure formatting and that comments are concise and informative for consumers.
    /// <summary>
    /// Represents a historical snapshot (revision) of a <see cref="Player"/>'s skill ratings.
    /// </summary>
    /// <remarks>
    /// Instances record which skill ratings were captured at a point in time and who reviewed
    /// or made the changes. The entity inherits auditing fields from <see cref="AuditableEntity"/>.
    /// </remarks>
    public class Revision : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the foreign key of the <see cref="Player"/> this revision belongs to.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// Navigation property to the <see cref="Player"/> whose skills are represented by this revision.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Optional comment provided by the reviewer or author of the revision.
        /// </summary>
        /// <remarks>
        /// Use this field to store contextual notes about why the revision was made or what changed.
        /// </remarks>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the <see cref="ApplicationUser"/> who reviewed or created this revision.
        /// </summary>
        public Guid ReviewerId { get; set; }

        /// <summary>
        /// Navigation property to the reviewer or user who created the revision.
        /// </summary>
        public ApplicationUser Reviewer { get; set; }

        /// <summary>
        /// Collection of <see cref="PlayerSkill"/> entries that were part of this revision.
        /// </summary>
        /// <remarks>
        /// Each <see cref="PlayerSkill"/> represents a rating for a specific <see cref="Skill"/>.
        /// The collection is initialized to a <see cref="HashSet{T}"/> to prevent null reference issues.
        /// </remarks>
        public ICollection<PlayerSkill> PlayerSkills { get; set; } = new HashSet<PlayerSkill>();
    }
}
