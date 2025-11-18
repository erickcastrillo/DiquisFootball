using Diquis.Application.Common.Identity.DTOs;
using Xunit;

namespace Diquis.Application.Tests.Common.Identity.DTOs
{
    public class ChangePasswordRequestValidatorTests
    {
        [Fact]
        public void Should_Have_Error_When_Fields_Are_Empty()
        {
            ChangePasswordRequestValidator validator = new ChangePasswordRequestValidator();
            ChangePasswordRequest request = new ChangePasswordRequest();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_Passwords_Do_Not_Match()
        {
            ChangePasswordRequestValidator validator = new ChangePasswordRequestValidator();
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                Password = "old",
                NewPassword = "new",
                ConfirmNewPassword = "different"
            };
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid()
        {
            ChangePasswordRequestValidator validator = new ChangePasswordRequestValidator();
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                Password = "old",
                NewPassword = "new",
                ConfirmNewPassword = "new"
            };
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }
    }
}
