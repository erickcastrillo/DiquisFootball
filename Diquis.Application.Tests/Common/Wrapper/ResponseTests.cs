using Diquis.Application.Common.Wrapper;

namespace Diquis.Application.Tests.Common.Wrapper
{
    public class ResponseTests
    {
        [Fact]
        public void Success_ShouldReturnSucceededTrue()
        {
            Response response = Response.Success();
            Assert.True(response.Succeeded);
        }

        [Fact]
        public void Fail_ShouldReturnSucceededFalse()
        {
            Response response = Response.Fail();
            Assert.False(response.Succeeded);
        }

        [Fact]
        public void Fail_WithMessage_ShouldContainMessage()
        {
            Response response = Response.Fail("error");
            Assert.Contains("error", response.Messages);
        }

        [Fact]
        public void Fail_WithMessages_ShouldContainAllMessages()
        {
            List<string> messages = ["a", "b"];
            Response response = Response.Fail(messages);
            Assert.Equal(messages, response.Messages);
        }
    }

    public class ResponseGenericTests
    {
        [Fact]
        public void Success_WithData_ShouldReturnData()
        {
            Response<string> response = Response<string>.Success("data");
            Assert.True(response.Succeeded);
            Assert.Equal("data", response.Data);
        }

        [Fact]
        public void Fail_WithMessage_ShouldContainMessage()
        {
            Response<string> response = Response<string>.Fail("error");
            Assert.False(response.Succeeded);
            Assert.Contains("error", response.Messages);
        }
    }
}
