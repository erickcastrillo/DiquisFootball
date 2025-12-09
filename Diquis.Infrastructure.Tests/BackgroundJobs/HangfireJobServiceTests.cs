using System.Linq.Expressions;
using Diquis.Application.Common.BackgroundJobs;
using Diquis.Infrastructure.BackgroundJobs;
using Moq;

using Xunit;

namespace Diquis.Infrastructure.Tests.BackgroundJobs
{
    /// <summary>
    /// Contains unit tests for <see cref="HangfireJobService"/>.
    /// </summary>
    public class HangfireJobServiceTests
    {
        /// <summary>
        /// Verifies that <see cref="HangfireJobService.Enqueue"/> calls the job client wrapper's <c>Enqueue</c> method.
        /// </summary>
        [Fact]
        public void Enqueue_CallsJobClient()
        {
            // Arrange
            Mock<IJobClientWrapper> jobClientWrapperMock = new Mock<IJobClientWrapper>();
            Expression<Action> expr = () => Console.WriteLine("test");
            _ = jobClientWrapperMock.Setup(j => j.Enqueue(expr)).Returns("jobid");
            HangfireJobService service = new(jobClientWrapperMock.Object);

            // Act
            string result = service.Enqueue(expr);

            // Assert
            jobClientWrapperMock.Verify(j => j.Enqueue(expr), Times.Once);
            Assert.Equal("jobid", result);
        }
    }
}
