using Diquis.Domain.Entities.Catalog;
using Diquis.Domain.Entities.Football.Common;
using Xunit;

namespace Diquis.Domain.Tests.Entities.Catalog
{
    public class ProductTests
    {
        [Fact]
        public void Product_CanBeCreated()
        {
            // Arrange
            var productName = "Test Product";
            var productDescription = "Test Description";
            var productLocale = Locale.ENGLISH;

            // Act
            var product = new Product
            {
                Name = productName,
                Description = productDescription,
                Locale = productLocale
            };

            // Assert
            Assert.Equal(productName, product.Name);
            Assert.Equal(productDescription, product.Description);
            Assert.Equal(productLocale, product.Locale);
        }
    }
}
