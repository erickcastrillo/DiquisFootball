using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }


    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
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
