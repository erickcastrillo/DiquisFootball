using Diquis.Application.Common.Identity.DTOs;

namespace Diquis.Application.Tests.Common.Identity.DTOs
{
    public class ForgotPasswordValidatorTests
    {
        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            ForgotPasswordValidator validator = new ForgotPasswordValidator();
            ForgotPasswordRequest request = new ForgotPasswordRequest { Email = string.Empty };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            ForgotPasswordValidator validator = new ForgotPasswordValidator();
            ForgotPasswordRequest request = new ForgotPasswordRequest { Email = "notanemail" };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            ForgotPasswordValidator validator = new ForgotPasswordValidator();
            ForgotPasswordRequest request = new ForgotPasswordRequest { Email = "test@email.com" };
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }
    }
}
