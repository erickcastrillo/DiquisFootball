using Diquis.Application.Common.Mailer;
using Moq;

namespace Diquis.Application.Tests.Common.Mailer
{
    public class MailServiceTests
    {
        [Fact]
        public async Task SendAsync_CanBeCalled()
        {
            Mock<IMailService> serviceMock = new Mock<IMailService>();
            MailRequest request = new MailRequest();
            _ = serviceMock.Setup(s => s.SendAsync(request)).Returns(Task.CompletedTask);
            await serviceMock.Object.SendAsync(request);
            serviceMock.Verify(s => s.SendAsync(request), Times.Once);
        }
    }
}
