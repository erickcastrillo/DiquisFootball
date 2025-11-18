using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Tests.Entities.Football.PlayerManagement
{
    public class PlayerSkillTests
    {
        [Fact]
        public void CanSetAndGetPlayerId()
        {
            PlayerSkill playerSkill = new();
            Guid playerId = Guid.NewGuid();
            playerSkill.PlayerId = playerId;
            Assert.Equal(playerId, playerSkill.PlayerId);
        }

        [Fact]
        public void CanSetAndGetSkillId()
        {
            PlayerSkill playerSkill = new();
            Guid skillId = Guid.NewGuid();
            playerSkill.SkillId = skillId;
            Assert.Equal(skillId, playerSkill.SkillId);
        }

        [Fact]
        public void CanSetAndGetRevisionId()
        {
            PlayerSkill playerSkill = new();
            Guid? revisionId = Guid.NewGuid();
            playerSkill.RevisionId = revisionId;
            Assert.Equal(revisionId, playerSkill.RevisionId);

            playerSkill.RevisionId = null;
            Assert.Null(playerSkill.RevisionId);
        }

        [Fact]
        public void CanSetAndGetProficiencyLevel()
        {
            PlayerSkill playerSkill = new()
            {
                ProficiencyLevel = 85
            };
            Assert.Equal(85, playerSkill.ProficiencyLevel);
        }

        [Fact]
        public void CanSetAndGetPlayerNavigationProperty()
        {
            Player player = new() { FirstName = "Test", LastName = "Player" };
            PlayerSkill playerSkill = new() { Player = player };
            Assert.Equal(player, playerSkill.Player);
        }

        [Fact]
        public void CanSetAndGetSkillNavigationProperty()
        {
            Skill skill = new() { Name = "Dribbling", Locale = Locale.ENGLISH };
            PlayerSkill playerSkill = new() { Skill = skill };
            Assert.Equal(skill, playerSkill.Skill);
        }

        [Fact]
        public void CanSetAndGetRevisionNavigationProperty()
        {
            Revision revision = new() { Comment = "Initial rating" };
            PlayerSkill playerSkill = new() { Revision = revision };
            Assert.Equal(revision, playerSkill.Revision);
        }

        [Fact]
        public void PlayerSkillInheritsFromAuditableEntity()
        {
            var playerSkill = new PlayerSkill();
            Assert.IsAssignableFrom<AuditableEntity>(playerSkill);
        }
    }
}