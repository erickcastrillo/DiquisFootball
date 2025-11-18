using Diquis.Application.Common;
using Moq;
using System.Linq.Expressions;

namespace Diquis.Application.Tests.Common
{
    public class BackgroundJobServiceTests
    {
        [Fact]
        public void Enqueue_CanBeCalled()
        {
            Mock<IBackgroundJobService> serviceMock = new();
            Expression<Action> expr = () => Console.WriteLine("test");
            _ = serviceMock.Setup(s => s.Enqueue(expr)).Returns("jobid");
            string result = serviceMock.Object.Enqueue(expr);
            Assert.Equal("jobid", result);
        }
    }
}
