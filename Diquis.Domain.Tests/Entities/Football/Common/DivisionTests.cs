using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Tests.Entities.Football.Common
{
    /// <summary>
    /// Contains unit tests for the <see cref="Division"/> entity, verifying its construction,
    /// property accessors, player collection management, and inheritance.
    /// </summary>
    public class DivisionTests
    {
        /// <summary>
        /// Verifies that the <see cref="Division"/> constructor initializes the <see cref="Division.Players"/> collection.
        /// </summary>
        [Fact]
        public void ConstructorInitializesPlayersCollection()
        {
            Division division = new() { Name = "Test", Locale = Locale.ENGLISH };
            Assert.NotNull(division.Players);
            Assert.Empty(division.Players);
        }

        /// <summary>
        /// Ensures that the <see cref="Division.Name"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetName()
        {
            Division division = new() { Name = "Premier", Locale = Locale.ENGLISH };
            Assert.Equal("Premier", division.Name);
        }

        /// <summary>
        /// Ensures that the <see cref="Division.Locale"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetLocale()
        {
            Division division = new() { Name = "Primera", Locale = Locale.SPANISH };
            Assert.Equal(Locale.SPANISH, division.Locale);
        }

        /// <summary>
        /// Tests adding and removing <see cref="Player"/> instances from the <see cref="Division.Players"/> collection.
        /// </summary>
        [Fact]
        public void CanAddAndRemovePlayers()
        {
            Player player1 = new() { FirstName = "Alice", LastName = "Brown" };
            Player player2 = new() { FirstName = "Bob", LastName = "Smith" };
            Division division = new() { Name = "Youth", Locale = Locale.ENGLISH };

            division.Players.Add(player1);
            division.Players.Add(player2);

            Assert.Contains(player1, division.Players);
            Assert.Contains(player2, division.Players);
            Assert.Equal(2, division.Players.Count);

            _ = division.Players.Remove(player1);
            Assert.DoesNotContain(player1, division.Players);
            _ = Assert.Single(division.Players);
        }

        /// <summary>
        /// Verifies that <see cref="Division"/> inherits from <see cref="AuditableEntity"/>.
        /// </summary>
        [Fact]
        public void DivisionInheritsFromAuditableEntity()
        {
            Division division = new() { Name = "Test", Locale = Locale.ENGLISH };
            _ = Assert.IsType<AuditableEntity>(division, exactMatch: false);
        }
    }
}