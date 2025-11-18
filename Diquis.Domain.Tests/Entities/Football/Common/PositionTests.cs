using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Tests.Entities.Football.Common
{
    /// <summary>
    /// Contains unit tests for the <see cref="Position"/> entity, verifying its construction,
    /// property accessors, player collection management, and inheritance.
    /// </summary>
    public class PositionTests
    {
        /// <summary>
        /// Verifies that the <see cref="Position"/> constructor initializes the <see cref="Position.Players"/> collection.
        /// </summary>
        [Fact]
        public void ConstructorInitializesPlayersCollection()
        {
            Position position = new() { Name = "Forward", Locale = Locale.ENGLISH };
            Assert.NotNull(position.Players);
            Assert.Empty(position.Players);
        }

        /// <summary>
        /// Tests that the <see cref="Position.Name"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetName()
        {
            Position position = new() { Name = "Defender", Locale = Locale.ENGLISH };
            Assert.Equal("Defender", position.Name);
        }

        /// <summary>
        /// Tests that the <see cref="Position.Locale"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetLocale()
        {
            Position position = new() { Name = "Midfielder", Locale = Locale.SPANISH };
            Assert.Equal(Locale.SPANISH, position.Locale);
        }

        /// <summary>
        /// Verifies that players can be added to and removed from the <see cref="Position.Players"/> collection.
        /// </summary>
        [Fact]
        public void CanAddAndRemovePlayers()
        {
            Player player1 = new() { FirstName = "Alice", LastName = "Brown" };
            Player player2 = new() { FirstName = "Bob", LastName = "Smith" };
            Position position = new() { Name = "Goalkeeper", Locale = Locale.ENGLISH };

            position.Players.Add(player1);
            position.Players.Add(player2);

            Assert.Contains(player1, position.Players);
            Assert.Contains(player2, position.Players);
            Assert.Equal(2, position.Players.Count);

            _ = position.Players.Remove(player1);
            Assert.DoesNotContain(player1, position.Players);
            _ = Assert.Single(position.Players);
        }

        /// <summary>
        /// Ensures that <see cref="Position"/> inherits from <see cref="AuditableEntity"/>.
        /// </summary>
        [Fact]
        public void PositionInheritsFromAuditableEntity()
        {
            Position position = new() { Name = "Test", Locale = Locale.ENGLISH };
            _ = Assert.IsType<AuditableEntity>(position, exactMatch: false);
        }
    }
}