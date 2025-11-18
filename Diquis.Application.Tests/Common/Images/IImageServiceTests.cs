using Diquis.Application.Common.Images;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Diquis.Application.Tests.Common.Images
{
    public class ImageServiceTests
    {
        [Fact]
        public async Task AddImage_CanBeCalled()
        {
            Mock<IImageService> serviceMock = new Mock<IImageService>();
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            _ = serviceMock.Setup(s => s.AddImage(fileMock.Object, 100, 100)).ReturnsAsync("url");
            string result = await serviceMock.Object.AddImage(fileMock.Object, 100, 100);
            Assert.Equal("url", result);
        }

        [Fact]
        public async Task DeleteImage_CanBeCalled()
        {
            Mock<IImageService> serviceMock = new Mock<IImageService>();
            _ = serviceMock.Setup(s => s.DeleteImage("url")).ReturnsAsync("deleted");
            string result = await serviceMock.Object.DeleteImage("url");
            Assert.Equal("deleted", result);
        }
    }
}
