using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    /// <summary>
    /// Represents a request to reset a user's password.
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        public string Password { get; set; } = null!;
        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// </summary>
        public string ConfirmPassword { get; set; } = null!;
        /// <summary>
        /// Gets or sets the password reset token.
        /// </summary>
        public string Token { get; set; } = null!;
    }


    /// <summary>
    /// Validator for <see cref="ResetPasswordRequest"/>.
    /// </summary>
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordRequestValidator"/> class.
        /// </summary>
        public ResetPasswordRequestValidator()
        {
            _ = RuleFor(x => x.Token).NotEmpty();
            _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
            _ = RuleFor(p => p.Password)
                .NotEmpty();
            _ = RuleFor(p => p.ConfirmPassword)
                .NotEmpty();
            _ = RuleFor(p => p.ConfirmPassword)
                .Equal(p => p.Password)
                    .WithMessage("Passwords do not match.");
        }
    }
}
