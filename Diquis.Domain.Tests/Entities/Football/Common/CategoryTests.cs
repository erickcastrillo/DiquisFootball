using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Domain.Tests.Entities.Football.Common
{
    /// <summary>
    /// Contains unit tests for the <see cref="Category"/> entity, verifying its construction,
    /// property accessors, player collection management, and inheritance.
    /// </summary>
    public class CategoryTests
    {
        /// <summary>
        /// Verifies that the <see cref="Category"/> constructor initializes the <see cref="Category.Players"/> collection.
        /// </summary>
        [Fact]
        public void ConstructorInitializesPlayersCollection()
        {
            Category category = new() { Name = "Test", Locale = Locale.ENGLISH };
            Assert.NotNull(category.Players);
            Assert.Empty(category.Players);
        }

        /// <summary>
        /// Tests that the <see cref="Category.Name"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetName()
        {
            Category category = new() { Name = "U12", Locale = Locale.ENGLISH };
            Assert.Equal("U12", category.Name);
        }

        /// <summary>
        /// Tests that the <see cref="Category.Locale"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetLocale()
        {
            Category category = new() { Name = "Senior", Locale = Locale.SPANISH };
            Assert.Equal(Locale.SPANISH, category.Locale);
        }

        /// <summary>
        /// Verifies that players can be added to and removed from the <see cref="Category.Players"/> collection.
        /// </summary>
        [Fact]
        public void CanAddAndRemovePlayers()
        {
            Player player1 = new() { FirstName = "John", LastName = "Doe" };
            Player player2 = new() { FirstName = "Jane", LastName = "Smith" };
            Category category = new() { Name = "Women", Locale = Locale.ENGLISH };

            category.Players.Add(player1);
            category.Players.Add(player2);

            Assert.Contains(player1, category.Players);
            Assert.Contains(player2, category.Players);
            Assert.Equal(2, category.Players.Count);

            _ = category.Players.Remove(player1);
            Assert.DoesNotContain(player1, category.Players);
            _ = Assert.Single(category.Players);
        }

        /// <summary>
        /// Ensures that <see cref="Category"/> inherits from <see cref="AuditableEntity"/>.
        /// </summary>
        [Fact]
        public void CategoryInheritsFromAuditableEntity()
        {
            Category category = new() { Name = "Test", Locale = Locale.ENGLISH };
            _ = Assert.IsType<AuditableEntity>(category, exactMatch: false);
        }
    }
}
