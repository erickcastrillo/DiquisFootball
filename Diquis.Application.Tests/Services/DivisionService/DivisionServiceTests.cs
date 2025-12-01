using AutoMapper;
using Diquis.Application.Common;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.DivisionService;
using Diquis.Application.Services.DivisionService.DTOs;
using Diquis.Application.Services.DivisionService.Filters;
using Diquis.Domain.Entities.Football.Common;
using Ardalis.Specification;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Diquis.Application.Tests.Services.DivisionService
{
    public class DivisionServiceTests
    {
        private readonly Mock<IRepositoryAsync> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Diquis.Application.Services.DivisionService.DivisionService _divisionService;

        public DivisionServiceTests()
        {
            _repositoryMock = new Mock<IRepositoryAsync>();
            _mapperMock = new Mock<IMapper>();
            _divisionService = new Diquis.Application.Services.DivisionService.DivisionService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetDivisionAsync_WhenDivisionExists_ReturnsSuccessResponse()
        {
            // Arrange
            var divisionId = Guid.NewGuid();
            var divisionDto = new DivisionDTO { Id = divisionId, Name = "Test Division" };

            _repositoryMock.Setup(r => r.GetByIdAsync<Division, DivisionDTO, Guid>(divisionId, null, default))
                .ReturnsAsync(divisionDto);

            // Act
            var result = await _divisionService.GetDivisionAsync(divisionId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(divisionId, result.Data.Id);
        }

        [Fact]
        public async Task GetDivisionAsync_WhenDivisionDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var divisionId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync<Division, DivisionDTO, Guid>(divisionId, null, default))
                .ThrowsAsync(new Exception("Not Found"));

            // Act
            var result = await _divisionService.GetDivisionAsync(divisionId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task CreateDivisionAsync_WhenDivisionIsNew_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new CreateDivisionRequest { Name = "New Division" };
            var newDivision = new Division { Id = Guid.NewGuid(), Name = request.Name };

            _repositoryMock.Setup(r => r.ExistsAsync<Division, Guid>(It.IsAny<ISpecification<Division>>(), default))
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.CreateAsync<Division, Guid>(It.IsAny<Division>()))
                .ReturnsAsync(newDivision);
             _mapperMock.Setup(m => m.Map(request, It.IsAny<Division>())).Returns(newDivision);

            // Act
            var result = await _divisionService.CreateDivisionAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotEqual(Guid.Empty, result.Data);
        }

        [Fact]
        public async Task CreateDivisionAsync_WhenDivisionExists_ReturnsFailResponse()
        {
            // Arrange
            var request = new CreateDivisionRequest { Name = "Existing Division" };

            _repositoryMock.Setup(r => r.ExistsAsync<Division, Guid>(It.IsAny<ISpecification<Division>>(), default))
                .ReturnsAsync(true);

            // Act
            var result = await _divisionService.CreateDivisionAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Division already exists", result.Messages[0]);
        }

        [Fact]
        public async Task UpdateDivisionAsync_WhenDivisionExists_ReturnsSuccessResponse()
        {
            // Arrange
            var divisionId = Guid.NewGuid();
            var request = new UpdateDivisionRequest { Name = "Updated Division" };
            var existingDivision = new Division { Id = divisionId, Name = "Old Division" };

            _repositoryMock.Setup(r => r.GetByIdAsync<Division, Guid>(divisionId, null, default))
                .ReturnsAsync(existingDivision);
            _repositoryMock.Setup(r => r.UpdateAsync<Division, Guid>(It.IsAny<Division>()))
                .ReturnsAsync(existingDivision);
            _mapperMock.Setup(m => m.Map(request, existingDivision)).Returns(existingDivision);

            // Act
            var result = await _divisionService.UpdateDivisionAsync(request, divisionId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(divisionId, result.Data);
        }

        [Fact]
        public async Task UpdateDivisionAsync_WhenDivisionDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var divisionId = Guid.NewGuid();
            var request = new UpdateDivisionRequest { Name = "Updated Division" };

            _repositoryMock.Setup(r => r.GetByIdAsync<Division, Guid>(divisionId, null, default))
                .ReturnsAsync((Division)null);

            // Act
            var result = await _divisionService.UpdateDivisionAsync(request, divisionId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task DeleteDivisionAsync_WhenDivisionExists_ReturnsSuccessResponse()
        {
            // Arrange
            var divisionId = Guid.NewGuid();
            var divisionToDelete = new Division { Id = divisionId, Name = "Test Division" };

            _repositoryMock.Setup(r => r.RemoveByIdAsync<Division, Guid>(divisionId))
                .ReturnsAsync(divisionToDelete);

            // Act
            var result = await _divisionService.DeleteDivisionAsync(divisionId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(divisionId, result.Data);
        }

        [Fact]
        public async Task DeleteDivisionAsync_WhenDivisionDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var divisionId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.RemoveByIdAsync<Division, Guid>(divisionId))
                .ThrowsAsync(new Exception("Not Found"));

            // Act
            var result = await _divisionService.DeleteDivisionAsync(divisionId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not Found", result.Messages[0]);
        }

        [Fact]
        public async Task GetDivisionsAsync_ReturnsListOfDivisions()
        {
            // Arrange
            var divisions = new List<DivisionDTO> { new DivisionDTO(), new DivisionDTO() };
            _repositoryMock.Setup(r => r.GetListAsync<Division, DivisionDTO, Guid>(It.IsAny<ISpecification<Division>>(), default))
                .ReturnsAsync(divisions);

            // Act
            var result = await _divisionService.GetDivisionsAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, ((List<DivisionDTO>)result.Data).Count);
        }

        [Fact]
        public async Task GetDivisionsPaginatedAsync_ReturnsPaginatedDivisions()
        {
            // Arrange
            var filter = new DivisionTableFilter { PageNumber = 1, PageSize = 10 };
            var pagedResponse = new PaginatedResponse<DivisionDTO>(new List<DivisionDTO> { new DivisionDTO() }, 1, 1, 10);

            _repositoryMock.Setup(r => r.GetPaginatedResultsAsync<Division, DivisionDTO, Guid>(filter.PageNumber, filter.PageSize, It.IsAny<ISpecification<Division>>(), default))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _divisionService.GetDivisionsPaginatedAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
        }
    }
}
