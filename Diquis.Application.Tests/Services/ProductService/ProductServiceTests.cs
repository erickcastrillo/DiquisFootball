using AutoMapper;
using Diquis.Application.Common;
using Diquis.Application.Common.ExcelExport;
using Diquis.Application.Common.PdfExport;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.ProductService;
using Diquis.Application.Services.ProductService.DTOs;
using Diquis.Application.Services.ProductService.Filters;
using Diquis.Domain.Entities.Catalog;
using Ardalis.Specification;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Diquis.Application.Tests.Services.ProductService
{
    public class ProductServiceTests
    {
        private readonly Mock<IRepositoryAsync> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IExcelExportService> _excelExportServiceMock;
        private readonly Mock<IPdfExportService> _pdfExportServiceMock;
        private readonly Diquis.Application.Services.ProductService.ProductService _productService;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IRepositoryAsync>();
            _mapperMock = new Mock<IMapper>();
            _excelExportServiceMock = new Mock<IExcelExportService>();
            _pdfExportServiceMock = new Mock<IPdfExportService>();
            _productService = new Diquis.Application.Services.ProductService.ProductService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _excelExportServiceMock.Object,
                _pdfExportServiceMock.Object);
        }

        [Fact]
        public async Task GetProductAsync_WhenProductExists_ReturnsSuccessResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDto = new ProductDTO { Id = productId, Name = "Test Product" };
            _repositoryMock.Setup(r => r.GetByIdAsync<Product, ProductDTO, Guid>(productId, null, default)).ReturnsAsync(productDto);

            // Act
            var result = await _productService.GetProductAsync(productId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(productId, result.Data.Id);
        }

        [Fact]
        public async Task GetProductAsync_WhenProductDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync<Product, ProductDTO, Guid>(productId, null, default)).ThrowsAsync(new Exception("Not Found"));

            // Act
            var result = await _productService.GetProductAsync(productId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task CreateProductAsync_WhenProductIsNew_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new CreateProductRequest { Name = "New Product", Description = "Desc" };
            var newProduct = new Product { Id = Guid.NewGuid(), Name = request.Name };

            _repositoryMock.Setup(r => r.ExistsAsync<Product, Guid>(It.IsAny<ISpecification<Product>>(), default)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.CreateAsync<Product, Guid>(It.IsAny<Product>())).ReturnsAsync(newProduct);
            _mapperMock.Setup(m => m.Map(request, It.IsAny<Product>())).Returns(newProduct);

            // Act
            var result = await _productService.CreateProductAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotEqual(Guid.Empty, result.Data);
        }

        [Fact]
        public async Task CreateProductAsync_WhenProductExists_ReturnsFailResponse()
        {
            // Arrange
            var request = new CreateProductRequest { Name = "Existing Product" };
            _repositoryMock.Setup(r => r.ExistsAsync<Product, Guid>(It.IsAny<ISpecification<Product>>(), default)).ReturnsAsync(true);

            // Act
            var result = await _productService.CreateProductAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Product already exists", result.Messages[0]);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenProductExists_ReturnsSuccessResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new UpdateProductRequest { Name = "Updated Product" };
            var existingProduct = new Product { Id = productId, Name = "Old Product" };

            _repositoryMock.Setup(r => r.GetByIdAsync<Product, Guid>(productId, null, default)).ReturnsAsync(existingProduct);
            _repositoryMock.Setup(r => r.UpdateAsync<Product, Guid>(It.IsAny<Product>())).ReturnsAsync(existingProduct);
            _mapperMock.Setup(m => m.Map(request, existingProduct)).Returns(existingProduct);

            // Act
            var result = await _productService.UpdateProductAsync(request, productId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(productId, result.Data);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenProductDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new UpdateProductRequest { Name = "Updated Product" };
            _repositoryMock.Setup(r => r.GetByIdAsync<Product, Guid>(productId, null, default)).ReturnsAsync((Product)null);

            // Act
            var result = await _productService.UpdateProductAsync(request, productId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductExists_ReturnsSuccessResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productToDelete = new Product { Id = productId };
            _repositoryMock.Setup(r => r.RemoveByIdAsync<Product, Guid>(productId)).ReturnsAsync(productToDelete);

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(productId, result.Data);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.RemoveByIdAsync<Product, Guid>(productId)).ThrowsAsync(new Exception("Not Found"));

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsListOfProducts()
        {
            // Arrange
            var products = new List<ProductDTO> { new ProductDTO(), new ProductDTO() };
            _repositoryMock.Setup(r => r.GetListAsync<Product, ProductDTO, Guid>(It.IsAny<ISpecification<Product>>(), default)).ReturnsAsync(products);

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, ((List<ProductDTO>)result.Data).Count);
        }

        [Fact]
        public async Task GetProductsPaginatedAsync_ReturnsPaginatedProducts()
        {
            // Arrange
            var filter = new ProductTableFilter { PageNumber = 1, PageSize = 10 };
            var pagedResponse = new PaginatedResponse<ProductDTO>(new List<ProductDTO> { new ProductDTO() }, 1, 1, 10);
            _repositoryMock.Setup(r => r.GetPaginatedResultsAsync<Product, ProductDTO, Guid>(filter.PageNumber, filter.PageSize, It.IsAny<ISpecification<Product>>(), default)).ReturnsAsync(pagedResponse);

            // Act
            var result = await _productService.GetProductsPaginatedAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetProductsExportAsync_ReturnsFileContent()
        {
            // Arrange
            var products = new List<ProductDTO> { new ProductDTO { Name = "Product 1" } };
            var fileBytes = new byte[] { 1, 2, 3 };
            _repositoryMock.Setup(r => r.GetListAsync<Product, ProductDTO, Guid>(It.IsAny<ISpecification<Product>>(), default)).ReturnsAsync(products);
            _excelExportServiceMock.Setup(s => s.ExportToExcel(products, It.IsAny<Dictionary<string, string>>(), "Products")).Returns(fileBytes);

            // Act
            var result = await _productService.GetProductsExportAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(fileBytes, result.Data);
        }

        [Fact]
        public async Task GetProductPdfAsync_WhenProductExists_ReturnsFileContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDto = new ProductDTO { Id = productId, Name = "Test Product" };
            var pdfBytes = new byte[] { 4, 5, 6 };
            _repositoryMock.Setup(r => r.GetByIdAsync<Product, ProductDTO, Guid>(productId, null, default)).ReturnsAsync(productDto);
            _pdfExportServiceMock.Setup(s => s.Export(productDto)).ReturnsAsync(pdfBytes);

            // Act
            var result = await _productService.GetProductPdfAsync(productId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(pdfBytes, result.Data);
        }

        [Fact]
        public async Task GetProductPdfAsync_WhenProductDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync<Product, ProductDTO, Guid>(productId, null, default)).ReturnsAsync((ProductDTO)null);

            // Act
            var result = await _productService.GetProductPdfAsync(productId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Product not found", result.Messages[0]);
        }
    }
}
