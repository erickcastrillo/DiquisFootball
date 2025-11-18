using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Domain.Entities.Football.PlayerManagement
{
    /// <summary>
    /// Represents a player's rating for a specific <see cref="Skill"/>.
    /// </summary>
    /// <remarks>
    /// Instances of this entity link a <see cref="Player"/> to a <see cref="Skill"/>
    /// together with a recorded proficiency level. The entity inherits from
    /// <see cref="AuditableEntity"/> to capture creation and modification metadata.
    /// A <see cref="PlayerSkill"/> may optionally be associated with a <see cref="Revision"/>
    /// to indicate the snapshot in which the rating was recorded.
    /// </remarks>
    public class PlayerSkill : AuditableEntity
    {
        /// <summary>
        /// Foreign key to the associated <see cref="Player"/>.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// Navigation property to the <see cref="Player"/> being rated.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Foreign key to the associated <see cref="Skill"/>.
        /// </summary>
        public Guid SkillId { get; set; }

        /// <summary>
        /// Navigation property to the <see cref="Skill"/> being rated.
        /// </summary>
        public Skill Skill { get; set; }

        /// <summary>
        /// Optional foreign key to the <see cref="Revision"/> where this rating was recorded.
        /// </summary>
        /// <remarks>
        /// This value is <c>null</c> when the rating is not tied to a specific revision snapshot.
        /// </remarks>
        public Guid? RevisionId { get; set; }

        /// <summary>
        /// Navigation property to the <see cref="Revision"/> that contains this rating.
        /// </summary>
        public Revision Revision { get; set; }

        /// <summary>
        /// Numeric value representing the player's proficiency for the referenced skill.
        /// </summary>
        /// <remarks>
        /// The interpretation (scale and meaning) of this value is defined by the domain
        /// (for example, a 1-10 or 0-100 scale). Consumers should interpret it consistently
        /// within the application context.
        /// </remarks>
        public int ProficiencyLevel { get; set; }
    }
}
