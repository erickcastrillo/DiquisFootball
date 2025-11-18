using Diquis.Application.Services.ProductService;
using Diquis.Application.Services.ProductService.DTOs;
using Diquis.Application.Services.ProductService.Filters;
using Diquis.Application.Common.Wrapper;
using Moq;

namespace Diquis.Application.Tests.Services.ProductService
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task GetProductsAsync_CanBeCalled()
        {
            Mock<IProductService> serviceMock = new();
            _ = serviceMock.Setup(s => s.GetProductsAsync(It.IsAny<string>())).ReturnsAsync(new Response<IEnumerable<ProductDTO>>());
            Response<IEnumerable<ProductDTO>> result = await serviceMock.Object.GetProductsAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetProductsPaginatedAsync_CanBeCalled()
        {
            Mock<IProductService> serviceMock = new();
            _ = serviceMock.Setup(s => s.GetProductsPaginatedAsync(It.IsAny<ProductTableFilter>())).ReturnsAsync(new PaginatedResponse<ProductDTO>(new List<ProductDTO>(), 0, 1, 10));
            PaginatedResponse<ProductDTO> result = await serviceMock.Object.GetProductsPaginatedAsync(new ProductTableFilter());
            Assert.NotNull(result);
        }
    }
}
