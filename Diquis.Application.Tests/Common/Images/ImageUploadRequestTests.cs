using Diquis.Application.Common.Images;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Diquis.Application.Tests.Common.Images
{
    public class ImageUploadRequestTests
    {
        [Fact]
        public void Can_Set_And_Get_Properties()
        {
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            ImageUploadRequest request = new ImageUploadRequest
            {
                ImageFile = fileMock.Object,
                DeleteCurrentImage = true
            };
            Assert.Equal(fileMock.Object, request.ImageFile);
            Assert.True(request.DeleteCurrentImage);
        }
    }
}
