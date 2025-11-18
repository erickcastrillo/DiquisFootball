using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    /// <summary>
    /// Request object for changing a user's password.
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// The current password.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// The new password.
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// The confirmation of the new password.
        /// </summary>
        public string ConfirmNewPassword { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="ChangePasswordRequest"/>.
    /// </summary>
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordRequestValidator"/> class.
        /// </summary>
        public ChangePasswordRequestValidator()
        {
            _ = RuleFor(p => p.Password)
                .NotEmpty();
            _ = RuleFor(p => p.NewPassword)
                .NotEmpty();
            _ = RuleFor(p => p.ConfirmNewPassword)
                .Equal(p => p.NewPassword)
                    .WithMessage("Passwords do not match.");
        }
    }
}
