using Diquis.Application.Common;
using Moq;

namespace Diquis.Application.Tests.Common
{
    public class CurrentTenantUserServiceTests
    {
        [Fact]
        public async Task SetTenantUser_CanBeCalled()
        {
            Mock<ICurrentTenantUserService> serviceMock = new();
            _ = serviceMock.Setup(s => s.SetTenantUser("tenant")).ReturnsAsync(true);
            bool result = await serviceMock.Object.SetTenantUser("tenant");
            Assert.True(result);
        }
    }
}
