using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    /// <summary>
    /// Request object for initiating a forgot password process.
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// The email address of the user requesting password reset.
        /// </summary>
        public string Email { get; set; } = null!;
    }

    /// <summary>
    /// Validator for <see cref="ForgotPasswordRequest"/>.
    /// </summary>
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordValidator"/> class.
        /// </summary>
        public ForgotPasswordValidator()
        {
            _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
