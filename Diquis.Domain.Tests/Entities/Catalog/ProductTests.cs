using Diquis.Domain.Entities.Catalog;
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

            // Act
            var product = new Product
            {
                Name = productName,
                Description = productDescription,
            };

            // Assert
            Assert.Equal(productName, product.Name);
            Assert.Equal(productDescription, product.Description);
        }
    }
}
