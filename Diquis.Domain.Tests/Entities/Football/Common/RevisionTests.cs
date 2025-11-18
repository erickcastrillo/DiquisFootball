using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Tests.Entities.Football.Common
{
    /// <summary>
    /// Contains unit tests for the <see cref="Revision"/> entity, verifying property behaviors,
    /// collection initialization, and inheritance from <see cref="AuditableEntity"/>.
    /// </summary>
    public class RevisionTests
    {
        /// <summary>
        /// Verifies that the <see cref="Revision"/> constructor initializes the <see cref="Revision.PlayerSkills"/> collection.
        /// </summary>
        [Fact]
        public void ConstructorInitializesPlayerSkillsCollection()
        {
            Revision revision = new();
            Assert.NotNull(revision.PlayerSkills);
            Assert.Empty(revision.PlayerSkills);
        }

        /// <summary>
        /// Tests that the <see cref="Revision.PlayerId"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetPlayerId()
        {
            Guid playerId = Guid.NewGuid();
            Revision revision = new() { PlayerId = playerId };
            Assert.Equal(playerId, revision.PlayerId);
        }

        /// <summary>
        /// Tests that the <see cref="Revision.Player"/> navigation property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetPlayer()
        {
            Player player = new() { FirstName = "Test", LastName = "Player" };
            Revision revision = new() { Player = player };
            Assert.Equal(player, revision.Player);
        }

        /// <summary>
        /// Tests that the <see cref="Revision.Comment"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetComment()
        {
            Revision revision = new() { Comment = "Reviewed for accuracy" };
            Assert.Equal("Reviewed for accuracy", revision.Comment);
        }

        /// <summary>
        /// Tests that the <see cref="Revision.ReviewerId"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetReviewerId()
        {
            Guid reviewerId = Guid.NewGuid();
            Revision revision = new() { ReviewerId = reviewerId };
            Assert.Equal(reviewerId, revision.ReviewerId);
        }

        /// <summary>
        /// Tests that the <see cref="Revision.Reviewer"/> navigation property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetReviewer()
        {
            ApplicationUser reviewer = new() { FirstName = "Alice", LastName = "Smith" };
            Revision revision = new() { Reviewer = reviewer };
            Assert.Equal(reviewer, revision.Reviewer);
        }

        /// <summary>
        /// Verifies that <see cref="PlayerSkill"/> entries can be added to and removed from the <see cref="Revision.PlayerSkills"/> collection.
        /// </summary>
        [Fact]
        public void CanAddAndRemovePlayerSkills()
        {
            PlayerSkill skill1 = new() { ProficiencyLevel = 5 };
            PlayerSkill skill2 = new() { ProficiencyLevel = 8 };
            Revision revision = new();

            revision.PlayerSkills.Add(skill1);
            revision.PlayerSkills.Add(skill2);

            Assert.Contains(skill1, revision.PlayerSkills);
            Assert.Contains(skill2, revision.PlayerSkills);
            Assert.Equal(2, revision.PlayerSkills.Count);

            _ = revision.PlayerSkills.Remove(skill1);
            Assert.DoesNotContain(skill1, revision.PlayerSkills);
            _ = Assert.Single(revision.PlayerSkills);
        }

        /// <summary>
        /// Ensures that <see cref="Revision"/> inherits from <see cref="AuditableEntity"/>.
        /// </summary>
        [Fact]
        public void RevisionInheritsFromAuditableEntity()
        {
            Revision revision = new();
            _ = Assert.IsType<AuditableEntity>(revision, exactMatch: false);
        }
    }
}