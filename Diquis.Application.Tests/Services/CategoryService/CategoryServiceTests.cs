using AutoMapper;
using Diquis.Application.Common;
using Diquis.Application.Services.CategoryService;
using Diquis.Application.Services.CategoryService.DTOs;
using Diquis.Domain.Entities.Football.Common;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Diquis.Application.Tests.Services.CategoryService
{
    public class CategoryServiceTests
    {
        private readonly Mock<IRepositoryAsync> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Diquis.Application.Services.CategoryService.CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _repositoryMock = new Mock<IRepositoryAsync>();
            _mapperMock = new Mock<IMapper>();
            _categoryService = new Diquis.Application.Services.CategoryService.CategoryService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetCategoryAsync_WhenCategoryExists_ReturnsSuccessResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var categoryDto = new CategoryDTO { Id = categoryId, Name = "Test Category" };

            _repositoryMock.Setup(r => r.GetByIdAsync<Category, CategoryDTO, Guid>(categoryId, null, default))
                .ReturnsAsync(categoryDto);

            // Act
            var result = await _categoryService.GetCategoryAsync(categoryId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(categoryId, result.Data.Id);
        }

        [Fact]
        public async Task GetCategoryAsync_WhenCategoryDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync<Category, CategoryDTO, Guid>(categoryId, null, default))
                .ThrowsAsync(new Exception("Not Found"));

            // Act
            var result = await _categoryService.GetCategoryAsync(categoryId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task CreateCategoryAsync_WhenCategoryIsNew_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new CreateCategoryRequest { Name = "New Category" };
            var newCategory = new Category { Id = Guid.NewGuid(), Name = request.Name };

            _repositoryMock.Setup(r => r.ExistsAsync<Category, Guid>(It.IsAny<ISpecification<Category>>(), default))
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.CreateAsync<Category, Guid>(It.IsAny<Category>()))
                .ReturnsAsync(newCategory);
            _mapperMock.Setup(m => m.Map(request, It.IsAny<Category>())).Returns(newCategory);


            // Act
            var result = await _categoryService.CreateCategoryAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotEqual(Guid.Empty, result.Data);
        }

        [Fact]
        public async Task CreateCategoryAsync_WhenCategoryExists_ReturnsFailResponse()
        {
            // Arrange
            var request = new CreateCategoryRequest { Name = "Existing Category" };

            _repositoryMock.Setup(r => r.ExistsAsync<Category, Guid>(It.IsAny<ISpecification<Category>>(), default))
                .ReturnsAsync(true);

            // Act
            var result = await _categoryService.CreateCategoryAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Category already exists", result.Messages[0]);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WhenCategoryExists_ReturnsSuccessResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var request = new UpdateCategoryRequest { Name = "Updated Category" };
            var existingCategory = new Category { Id = categoryId, Name = "Old Category" };

            _repositoryMock.Setup(r => r.GetByIdAsync<Category, Guid>(categoryId, null, default))
                .ReturnsAsync(existingCategory);
            _repositoryMock.Setup(r => r.UpdateAsync<Category, Guid>(It.IsAny<Category>()))
                .ReturnsAsync(existingCategory);
            _mapperMock.Setup(m => m.Map(request, existingCategory)).Returns(existingCategory);

            // Act
            var result = await _categoryService.UpdateCategoryAsync(request, categoryId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(categoryId, result.Data);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WhenCategoryDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var request = new UpdateCategoryRequest { Name = "Updated Category" };

            _repositoryMock.Setup(r => r.GetByIdAsync<Category, Guid>(categoryId, null, default))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _categoryService.UpdateCategoryAsync(request, categoryId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenCategoryExists_ReturnsSuccessResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var categoryToDelete = new Category { Id = categoryId, Name = "Test Category" };

            _repositoryMock.Setup(r => r.RemoveByIdAsync<Category, Guid>(categoryId))
                .ReturnsAsync(categoryToDelete);

            // Act
            var result = await _categoryService.DeleteCategoryAsync(categoryId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(categoryId, result.Data);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenCategoryDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.RemoveByIdAsync<Category, Guid>(categoryId))
                .ThrowsAsync(new Exception("Not Found"));

            // Act
            var result = await _categoryService.DeleteCategoryAsync(categoryId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsListOfCategories()
        {
            // Arrange
            var categories = new List<CategoryDTO> { new CategoryDTO(), new CategoryDTO() };
            _repositoryMock.Setup(r => r.GetListAsync<Category, CategoryDTO, Guid>(It.IsAny<ISpecification<Category>>(), default))
                .ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetCategoriesAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, ((List<CategoryDTO>)result.Data).Count);
        }

        [Fact]
        public async Task GetCategoriesPaginatedAsync_ReturnsPaginatedCategories()
        {
            // Arrange
            var filter = new Diquis.Application.Services.CategoryService.Filters.CategoryTableFilter { PageNumber = 1, PageSize = 10 };
            var pagedResponse = new PaginatedResponse<CategoryDTO>(new List<CategoryDTO> { new CategoryDTO() }, 1, 1, 10);

            _repositoryMock.Setup(r => r.GetPaginatedResultsAsync<Category, CategoryDTO, Guid>(filter.PageNumber, filter.PageSize, It.IsAny<ISpecification<Category>>(), default))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _categoryService.GetCategoriesPaginatedAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
        }
    }
}
