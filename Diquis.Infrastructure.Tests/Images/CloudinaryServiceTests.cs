using System.Threading.Tasks;
using Diquis.Infrastructure.Images;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Diquis.Infrastructure.Tests.Images
{
    public class CloudinaryServiceTests
    {
        [Fact]
        public void Constructor_SetsCloudinary()
        {
            Mock<IOptions<CloudinarySettings>> optionsMock = new Mock<IOptions<CloudinarySettings>>();
            optionsMock.Setup(o => o.Value).Returns(new CloudinarySettings
            {
                CloudName = "test",
                ApiKey = "key",
                ApiSecret = "secret"
            });
            CloudinaryService service = new CloudinaryService(optionsMock.Object);
            Assert.NotNull(service);
        }
    }
}
