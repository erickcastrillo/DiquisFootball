using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Tests.Entities.Football.Common
{
    /// <summary>
    /// Contains unit tests for the <see cref="Skill"/> entity, verifying its construction,
    /// property accessors, collection management, and inheritance.
    /// </summary>
    public class SkillTests
    {
        /// <summary>
        /// Verifies that the <see cref="Skill"/> constructor initializes the <see cref="Skill.PlayerSkills"/> collection.
        /// </summary>
        [Fact]
        public void ConstructorInitializesPlayerSkillsCollection()
        {
            Skill skill = new() { Name = "Dribbling", Locale = Locale.ENGLISH };
            Assert.NotNull(skill.PlayerSkills);
            Assert.Empty(skill.PlayerSkills);
        }

        /// <summary>
        /// Ensures that the <see cref="Skill.Name"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetName()
        {
            Skill skill = new() { Name = "Passing", Locale = Locale.ENGLISH };
            Assert.Equal("Passing", skill.Name);
        }

        /// <summary>
        /// Ensures that the <see cref="Skill.Locale"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetLocale()
        {
            Skill skill = new() { Name = "Shooting", Locale = Locale.SPANISH };
            Assert.Equal(Locale.SPANISH, skill.Locale);
        }

        /// <summary>
        /// Tests adding and removing <see cref="PlayerSkill"/> instances from the <see cref="Skill.PlayerSkills"/> collection.
        /// </summary>
        [Fact]
        public void CanAddAndRemovePlayerSkills()
        {
            PlayerSkill ps1 = new();
            PlayerSkill ps2 = new();
            Skill skill = new() { Name = "Tackling", Locale = Locale.ENGLISH };

            skill.PlayerSkills.Add(ps1);
            skill.PlayerSkills.Add(ps2);

            Assert.Contains(ps1, skill.PlayerSkills);
            Assert.Contains(ps2, skill.PlayerSkills);
            Assert.Equal(2, skill.PlayerSkills.Count);

            _ = skill.PlayerSkills.Remove(ps1);
            Assert.DoesNotContain(ps1, skill.PlayerSkills);
            _ = Assert.Single(skill.PlayerSkills);
        }

        /// <summary>
        /// Verifies that <see cref="Skill"/> inherits from <see cref="AuditableEntity"/>.
        /// </summary>
        [Fact]
        public void SkillInheritsFromAuditableEntity()
        {
            Skill skill = new() { Name = "Test", Locale = Locale.ENGLISH };
            _ = Assert.IsType<AuditableEntity>(skill, exactMatch: false);
        }
    }
}