using Diquis.Domain.Entities.Football.PlayerManagement;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Common;

namespace Diquis.Domain.Tests.Entities.Football.PlayerManagement
{
    public class PlayerTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            var player = new Player
            {
                FirstName = "Lionel",
                LastName = "Messi",
                DateOfBirth = new DateTime(1987, 6, 24),
                PreferredFoot = Foot.Left,
                JerseyNumber = 10,
                HeightInCm = 170.5m,
                WeightInKg = 72.3m,
                MedicalNotes = "No known issues",
                GuardianId = Guid.NewGuid(),
                Guardian = new ApplicationUser { FirstName = "Jorge" },
                PositionId = Guid.NewGuid(),
                Position = new Position { Name = "Forward" }
            };

            Assert.Equal("Lionel", player.FirstName);
            Assert.Equal("Messi", player.LastName);
            Assert.Equal(new DateTime(1987, 6, 24), player.DateOfBirth);
            Assert.Equal(Foot.Left, player.PreferredFoot);
            Assert.Equal(10, player.JerseyNumber);
            Assert.Equal(170.5m, player.HeightInCm);
            Assert.Equal(72.3m, player.WeightInKg);
            Assert.Equal("No known issues", player.MedicalNotes);
            Assert.NotNull(player.Guardian);
            Assert.NotNull(player.Position);
        }

        [Fact]
        public void RevisionsCollection_InitializesAndCanAddRemove()
        {
            var player = new Player();
            Assert.NotNull(player.Revisions);
            Assert.Empty(player.Revisions);

            var revision = new Revision { CreatedOn = DateTime.UtcNow };
            player.Revisions.Add(revision);
            Assert.Contains(revision, player.Revisions);

            player.Revisions.Remove(revision);
            Assert.Empty(player.Revisions);
        }

        [Fact]
        public void LastRevision_ReturnsNullIfNoRevisions()
        {
            var player = new Player();
            Assert.Null(player.LastRevision());
        }

        [Fact]
        public void LastRevision_ReturnsMostRecentRevision()
        {
            var rev1 = new Revision { CreatedOn = new DateTime(2020, 1, 1) };
            var rev2 = new Revision { CreatedOn = new DateTime(2021, 1, 1) };
            var player = new Player();
            player.Revisions.Add(rev1);
            player.Revisions.Add(rev2);

            var last = player.LastRevision();
            Assert.Equal(rev2, last);
        }

        [Fact]
        public void CurrentPlayerSkills_ReturnsEmptyIfNoRevisions()
        {
            var player = new Player();
            Assert.Empty(player.CurrentPlayerSkills);
        }

        [Fact]
        public void CurrentPlayerSkills_ReturnsSkillsFromLastRevision()
        {
            var skill1 = new PlayerSkill { ProficiencyLevel = 80 };
            var skill2 = new PlayerSkill { ProficiencyLevel = 90 };
            var rev = new Revision { CreatedOn = DateTime.UtcNow };
            rev.PlayerSkills = new List<PlayerSkill> { skill1, skill2 };
            var player = new Player();
            player.Revisions.Add(rev);

            var skills = player.CurrentPlayerSkills;
            Assert.Contains(skill1, skills);
            Assert.Contains(skill2, skills);
        }

        [Fact]
        public void ReadableAge_ReturnsYearsAndMonths()
        {
            var today = DateTime.Today;
            var dob = today.AddYears(-20).AddMonths(-5);
            var player = new Player { DateOfBirth = dob };
            var result = player.ReadableAge();
            Assert.Contains("20 year", result);
            Assert.Contains("5 month", result);
        }

        [Fact]
        public void ReadableAge_ReturnsDaysIfLessThanOneMonth()
        {
            var today = DateTime.Today;
            var dob = today.AddDays(-12);
            var player = new Player { DateOfBirth = dob };
            var result = player.ReadableAge();
            Assert.Contains("12 day", result);
        }

        [Fact]
        public void ReadableAge_ReturnsZeroDaysIfBirthInFuture()
        {
            var player = new Player { DateOfBirth = DateTime.Today.AddDays(1) };
            var result = player.ReadableAge();
            Assert.Equal("0 days", result);
        }

        [Fact]
        public void ReadableAge_HandlesExactOneYearAndMonth()
        {
            var today = DateTime.Today;
            var dob = today.AddYears(-1).AddMonths(-1);
            var player = new Player { DateOfBirth = dob };
            var result = player.ReadableAge();
            Assert.Contains("1 year", result);
            Assert.Contains("1 month", result);
        }

        [Fact]
        public void ReadableAge_HandlesPluralization()
        {
            var today = DateTime.Today;
            var dob = today.AddYears(-2).AddMonths(-2);
            var player = new Player { DateOfBirth = dob };
            var result = player.ReadableAge();
            Assert.Contains("2 years", result);
            Assert.Contains("2 months", result);
        }
    }
}