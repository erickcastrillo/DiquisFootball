using Diquis.Application.Common.Wrapper;

namespace Diquis.Application.Tests.Common.Wrapper
{
    public class PaginatedResponseTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            List<int> data = new List<int> { 1, 2, 3 };
            PaginatedResponse<int> response = new PaginatedResponse<int>(data, 10, 2, 3);
            Assert.Equal(data, response.Data);
            Assert.Equal(2, response.CurrentPage);
            Assert.Equal(3, response.PageSize);
            Assert.Equal(4, response.TotalPages);
            Assert.Equal(10, response.TotalCount);
        }

        [Fact]
        public void HasPreviousPage_ShouldBeTrue_WhenCurrentPageGreaterThan1()
        {
            PaginatedResponse<int> response = new PaginatedResponse<int>(new List<int>(), 10, 2, 3);
            Assert.True(response.HasPreviousPage);
        }

        [Fact]
        public void HasNextPage_ShouldBeTrue_WhenCurrentPageLessThanTotalPages()
        {
            PaginatedResponse<int> response = new PaginatedResponse<int>(new List<int>(), 10, 2, 3);
            Assert.True(response.HasNextPage);
        }
    }
}
