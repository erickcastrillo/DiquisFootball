using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Entities.Football.Common
{
    /// <summary>
    /// Represents a football skill that can be assigned to players and rated.
    /// </summary>
    /// <remarks>
    /// Each <see cref="Skill"/> is uniquely identified by its <see cref="Name"/> and <see cref="Locale"/>.
    /// The entity supports many-to-many relationships with players through <see cref="PlayerSkill"/>.
    /// Inherits auditing and tenant information from <see cref="AuditableEntity"/>.
    /// </remarks>
    public class Skill : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the skill (e.g., "Dribbling", "Passing").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the locale for the skill name, indicating the language or region.
        /// </summary>
        public Locale Locale { get; set; }

        /// <summary>
        /// Inverse navigation property for the many-to-many relationship with players through <see cref="PlayerSkill"/>.
        /// </summary>
        /// <remarks>
        /// Contains all <see cref="PlayerSkill"/> entries that reference this skill.
        /// The collection is initialized to a <see cref="HashSet{T}"/> to prevent null reference issues.
        /// </remarks>
        public ICollection<PlayerSkill> PlayerSkills { get; set; } = new HashSet<PlayerSkill>();
    }
}
